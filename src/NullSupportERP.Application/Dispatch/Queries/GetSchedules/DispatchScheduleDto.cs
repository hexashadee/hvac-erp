namespace NullSupportERP.Application.Dispatch.Queries.GetSchedules;

public sealed record DispatchScheduleDto(
    Guid Id,
    Guid WorkOrderId,
    DateTimeOffset ScheduledStartUtc,
    DateTimeOffset ScheduledEndUtc,
    DateTimeOffset ArrivalWindowStartUtc,
    DateTimeOffset ArrivalWindowEndUtc,
    string TimeZoneId,
    string? DispatchNotes,
    string Status,
    Guid? PrimaryTechnicianUserId,
    IReadOnlyList<DispatchScheduleTechnicianDto> Technicians);
