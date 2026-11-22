using NullSupportERP.Application.Dispatch.Queries.GetSchedules;
using NullSupportERP.Domain.Dispatch;
using NullSupportERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace NullSupportERP.Infrastructure.Dispatch.Queries;

internal sealed class DispatchScheduleQueries
    : IDispatchScheduleQueries
{
    private readonly NullSupportErpDbContext _dbContext;

    public DispatchScheduleQueries(
        NullSupportErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<DispatchScheduleDto>>
        GetSchedulesAsync(
            CancellationToken cancellationToken = default)
    {
        var schedules =
            await _dbContext.WorkOrderSchedules
                .AsNoTracking()
                .OrderBy(
                    schedule =>
                        schedule.ScheduledStartUtc)
                .Select(
                    schedule =>
                        new
                        {
                            schedule.Id,
                            schedule.WorkOrderId,
                            schedule.ScheduledStartUtc,
                            schedule.ScheduledEndUtc,
                            schedule.ArrivalWindowStartUtc,
                            schedule.ArrivalWindowEndUtc,
                            schedule.TimeZoneId,
                            schedule.DispatchNotes,
                            schedule.Status
                        })
                .ToListAsync(
                    cancellationToken);

        if (schedules.Count == 0)
        {
            return [];
        }

        Guid[] scheduleIds =
            schedules
                .Select(schedule => schedule.Id)
                .ToArray();

        var technicianRows =
            await _dbContext
                .WorkOrderScheduleTechnicians
                .AsNoTracking()
                .Where(
                    technician =>
                        scheduleIds.Contains(
                            technician.WorkOrderScheduleId))
                .OrderBy(
                    technician =>
                        technician.AssignedAtUtc)
                .Select(
                    technician =>
                        new
                        {
                            technician.Id,
                            technician.WorkOrderScheduleId,
                            technician.TechnicianUserId,
                            technician.Role,
                            technician.AssignedAtUtc
                        })
                .ToListAsync(
                    cancellationToken);

        Dictionary<
            Guid,
            List<DispatchScheduleTechnicianDto>>
            techniciansBySchedule =
                technicianRows
                    .GroupBy(
                        technician =>
                            technician.WorkOrderScheduleId)
                    .ToDictionary(
                        group => group.Key,
                        group =>
                            group
                                .Select(
                                    technician =>
                                        new DispatchScheduleTechnicianDto(
                                            technician.Id,
                                            technician.TechnicianUserId,
                                            technician.Role.ToString(),
                                            technician.AssignedAtUtc))
                                .ToList());

        Dictionary<Guid, Guid>
            primaryTechniciansBySchedule =
                technicianRows
                    .Where(
                        technician =>
                            technician.Role ==
                            WorkOrderTechnicianRole.Primary)
                    .ToDictionary(
                        technician =>
                            technician.WorkOrderScheduleId,
                        technician =>
                            technician.TechnicianUserId);

        return schedules
            .Select(
                schedule =>
                {
                    IReadOnlyList<
                        DispatchScheduleTechnicianDto>
                        technicians =
                            techniciansBySchedule.TryGetValue(
                                schedule.Id,
                                out List<
                                    DispatchScheduleTechnicianDto>?
                                    assignedTechnicians)
                                ? assignedTechnicians
                                : [];

                    Guid? primaryTechnicianUserId =
                        primaryTechniciansBySchedule.TryGetValue(
                            schedule.Id,
                            out Guid technicianUserId)
                            ? technicianUserId
                            : null;

                    return new DispatchScheduleDto(
                        schedule.Id,
                        schedule.WorkOrderId,
                        schedule.ScheduledStartUtc,
                        schedule.ScheduledEndUtc,
                        schedule.ArrivalWindowStartUtc,
                        schedule.ArrivalWindowEndUtc,
                        schedule.TimeZoneId,
                        schedule.DispatchNotes,
                        schedule.Status.ToString(),
                        primaryTechnicianUserId,
                        technicians);
                })
            .ToArray();
    }
}
