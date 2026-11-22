namespace NullSupportERP.Application.Dispatch.Queries.GetSchedules;

public sealed record DispatchScheduleTechnicianDto(
    Guid AssignmentId,
    Guid TechnicianUserId,
    string Role,
    DateTimeOffset AssignedAtUtc);
