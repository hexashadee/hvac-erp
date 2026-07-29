namespace HvacERP.Domain.Dispatch;

public sealed class WorkOrderScheduleTechnician
{
    public Guid Id { get; private set; }

    public Guid WorkOrderScheduleId { get; private set; }

    public Guid TechnicianUserId { get; private set; }

    public WorkOrderTechnicianRole Role { get; private set; }

    public DateTimeOffset AssignedAtUtc { get; private set; }

    public Guid AssignedByUserId { get; private set; }

    public DateTimeOffset? LastModifiedAtUtc { get; private set; }

    public Guid? LastModifiedByUserId { get; private set; }

    private WorkOrderScheduleTechnician(
        Guid id,
        Guid workOrderScheduleId,
        Guid technicianUserId,
        WorkOrderTechnicianRole role,
        DateTimeOffset assignedAtUtc,
        Guid assignedByUserId)
    {
        Id = id;
        WorkOrderScheduleId = workOrderScheduleId;
        TechnicianUserId = technicianUserId;
        Role = role;
        AssignedAtUtc = assignedAtUtc;
        AssignedByUserId = assignedByUserId;
    }

    internal static WorkOrderScheduleTechnician Create(
        Guid id,
        Guid workOrderScheduleId,
        Guid technicianUserId,
        WorkOrderTechnicianRole role,
        DateTimeOffset assignedAtUtc,
        Guid assignedByUserId)
    {
        ValidateId(
            id,
            nameof(id),
            "Schedule technician assignment ID");

        ValidateId(
            workOrderScheduleId,
            nameof(workOrderScheduleId),
            "Work order schedule ID");

        ValidateId(
            technicianUserId,
            nameof(technicianUserId),
            "Technician user ID");

        ValidateRole(role);

        ValidateId(
            assignedByUserId,
            nameof(assignedByUserId),
            "Assigned by user ID");

        return new WorkOrderScheduleTechnician(
            id,
            workOrderScheduleId,
            technicianUserId,
            role,
            assignedAtUtc,
            assignedByUserId);
    }

    internal void ChangeRole(
        WorkOrderTechnicianRole role,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateRole(role);

        ValidateId(
            modifiedByUserId,
            nameof(modifiedByUserId),
            "Modified by user ID");

        if (modifiedAtUtc < AssignedAtUtc)
        {
            throw new ArgumentException(
                "A technician assignment cannot be modified before it was created.",
                nameof(modifiedAtUtc));
        }

        Role = role;
        LastModifiedAtUtc = modifiedAtUtc;
        LastModifiedByUserId = modifiedByUserId;
    }

    private static void ValidateRole(
        WorkOrderTechnicianRole role)
    {
        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(
                nameof(role),
                role,
                "Work order technician role is invalid.");
        }
    }

    private static void ValidateId(
        Guid value,
        string parameterName,
        string displayName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(
                $"{displayName} is required.",
                parameterName);
        }
    }
}
