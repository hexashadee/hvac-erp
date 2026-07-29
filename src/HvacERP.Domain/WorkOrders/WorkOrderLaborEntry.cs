namespace HvacERP.Domain.WorkOrders;

public sealed class WorkOrderLaborEntry
{
    public Guid Id { get; private set; }

    public Guid TechnicianUserId { get; private set; }

    public WorkOrderLaborType Type { get; private set; }

    public DateTimeOffset StartedAtUtc { get; private set; }

    public DateTimeOffset EndedAtUtc { get; private set; }

    public bool IsBillable { get; private set; }

    public decimal HourlyRate { get; private set; }

    public string? Notes { get; private set; }

    public decimal DurationHours =>
        decimal.Round(
            (decimal)(EndedAtUtc - StartedAtUtc).TotalHours,
            2,
            MidpointRounding.AwayFromZero);

    public decimal BillableAmount =>
        IsBillable
            ? decimal.Round(
                DurationHours * HourlyRate,
                2,
                MidpointRounding.AwayFromZero)
            : 0m;

    internal WorkOrderLaborEntry(
        Guid id,
        Guid technicianUserId,
        WorkOrderLaborType type,
        DateTimeOffset startedAtUtc,
        DateTimeOffset endedAtUtc,
        bool isBillable,
        decimal hourlyRate,
        string? notes)
    {
        Id = id;
        TechnicianUserId = technicianUserId;
        Type = type;
        StartedAtUtc = startedAtUtc;
        EndedAtUtc = endedAtUtc;
        IsBillable = isBillable;
        HourlyRate = hourlyRate;
        Notes = notes;
    }

    internal void Update(
        Guid technicianUserId,
        WorkOrderLaborType type,
        DateTimeOffset startedAtUtc,
        DateTimeOffset endedAtUtc,
        bool isBillable,
        decimal hourlyRate,
        string? notes)
    {
        TechnicianUserId = technicianUserId;
        Type = type;
        StartedAtUtc = startedAtUtc;
        EndedAtUtc = endedAtUtc;
        IsBillable = isBillable;
        HourlyRate = hourlyRate;
        Notes = notes;
    }
}
