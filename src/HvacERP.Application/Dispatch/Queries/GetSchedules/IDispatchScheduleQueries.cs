namespace HvacERP.Application.Dispatch.Queries.GetSchedules;

public interface IDispatchScheduleQueries
{
    Task<IReadOnlyList<DispatchScheduleDto>> GetSchedulesAsync(
        CancellationToken cancellationToken = default);
}
