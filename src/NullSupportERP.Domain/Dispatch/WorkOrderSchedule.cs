using NullSupportERP.Domain.Common.Base;

namespace NullSupportERP.Domain.Dispatch;

public sealed class WorkOrderSchedule : AggregateRoot
{
    public const int MaximumTimeZoneIdLength = 100;

    public const int MaximumDispatchNotesLength = 2_000;

    public const int MaximumCancellationReasonLength = 500;

    private readonly List<WorkOrderScheduleTechnician> _technicians = [];

    public Guid WorkOrderId { get; private set; }

    public DateTimeOffset ScheduledStartUtc { get; private set; }

    public DateTimeOffset ScheduledEndUtc { get; private set; }

    public DateTimeOffset ArrivalWindowStartUtc { get; private set; }

    public DateTimeOffset ArrivalWindowEndUtc { get; private set; }

    public string TimeZoneId { get; private set; }

    public string? DispatchNotes { get; private set; }

    public WorkOrderScheduleStatus Status { get; private set; }

    public IReadOnlyCollection<WorkOrderScheduleTechnician> Technicians =>
        _technicians
            .OrderBy(technician => technician.Role)
            .ThenBy(technician => technician.AssignedAtUtc)
            .ToArray();

    public Guid? PrimaryTechnicianUserId =>
        _technicians
            .SingleOrDefault(
                technician =>
                    technician.Role == WorkOrderTechnicianRole.Primary)
            ?.TechnicianUserId;

    public DateTimeOffset? StartedAtUtc { get; private set; }

    public Guid? StartedByUserId { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public Guid? CompletedByUserId { get; private set; }

    public DateTimeOffset? CancelledAtUtc { get; private set; }

    public Guid? CancelledByUserId { get; private set; }

    public string? CancellationReason { get; private set; }

    public bool IsScheduled =>
        Status == WorkOrderScheduleStatus.Scheduled;

    public bool IsInProgress =>
        Status == WorkOrderScheduleStatus.InProgress;

    public bool IsCompleted =>
        Status == WorkOrderScheduleStatus.Completed;

    public bool IsCancelled =>
        Status == WorkOrderScheduleStatus.Cancelled;

    private WorkOrderSchedule(
        Guid id,
        Guid workOrderId,
        DateTimeOffset scheduledStartUtc,
        DateTimeOffset scheduledEndUtc,
        DateTimeOffset arrivalWindowStartUtc,
        DateTimeOffset arrivalWindowEndUtc,
        string timeZoneId,
        string? dispatchNotes,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(
            id,
            createdAtUtc,
            createdByUserId)
    {
        WorkOrderId = workOrderId;
        ScheduledStartUtc = scheduledStartUtc;
        ScheduledEndUtc = scheduledEndUtc;
        ArrivalWindowStartUtc = arrivalWindowStartUtc;
        ArrivalWindowEndUtc = arrivalWindowEndUtc;
        TimeZoneId = timeZoneId;
        DispatchNotes = dispatchNotes;
        Status = WorkOrderScheduleStatus.Scheduled;
    }

    public static WorkOrderSchedule Create(
        Guid id,
        Guid workOrderId,
        DateTimeOffset scheduledStartUtc,
        DateTimeOffset scheduledEndUtc,
        DateTimeOffset arrivalWindowStartUtc,
        DateTimeOffset arrivalWindowEndUtc,
        string timeZoneId,
        string? dispatchNotes,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        ValidateId(
            id,
            nameof(id),
            "Work order schedule ID");

        ValidateId(
            workOrderId,
            nameof(workOrderId),
            "Work order ID");

        ValidateScheduleWindow(
            scheduledStartUtc,
            scheduledEndUtc);

        ValidateArrivalWindow(
            arrivalWindowStartUtc,
            arrivalWindowEndUtc,
            scheduledStartUtc,
            scheduledEndUtc);

        string normalizedTimeZoneId =
            NormalizeRequired(
                timeZoneId,
                MaximumTimeZoneIdLength,
                nameof(timeZoneId),
                "Time-zone ID");

        string? normalizedDispatchNotes =
            NormalizeOptional(
                dispatchNotes,
                MaximumDispatchNotesLength,
                nameof(dispatchNotes),
                "Dispatch notes");

        ValidateId(
            createdByUserId,
            nameof(createdByUserId),
            "Created by user ID");

        return new WorkOrderSchedule(
            id,
            workOrderId,
            scheduledStartUtc,
            scheduledEndUtc,
            arrivalWindowStartUtc,
            arrivalWindowEndUtc,
            normalizedTimeZoneId,
            normalizedDispatchNotes,
            createdAtUtc,
            createdByUserId);
    }

    public void Reschedule(
        DateTimeOffset scheduledStartUtc,
        DateTimeOffset scheduledEndUtc,
        DateTimeOffset arrivalWindowStartUtc,
        DateTimeOffset arrivalWindowEndUtc,
        string timeZoneId,
        string? dispatchNotes,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureScheduled();

        ValidateScheduleWindow(
            scheduledStartUtc,
            scheduledEndUtc);

        ValidateArrivalWindow(
            arrivalWindowStartUtc,
            arrivalWindowEndUtc,
            scheduledStartUtc,
            scheduledEndUtc);

        string normalizedTimeZoneId =
            NormalizeRequired(
                timeZoneId,
                MaximumTimeZoneIdLength,
                nameof(timeZoneId),
                "Time-zone ID");

        string? normalizedDispatchNotes =
            NormalizeOptional(
                dispatchNotes,
                MaximumDispatchNotesLength,
                nameof(dispatchNotes),
                "Dispatch notes");

        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        ScheduledStartUtc = scheduledStartUtc;
        ScheduledEndUtc = scheduledEndUtc;
        ArrivalWindowStartUtc = arrivalWindowStartUtc;
        ArrivalWindowEndUtc = arrivalWindowEndUtc;
        TimeZoneId = normalizedTimeZoneId;
        DispatchNotes = normalizedDispatchNotes;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void UpdateDispatchNotes(
        string? dispatchNotes,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureActive();

        string? normalizedDispatchNotes =
            NormalizeOptional(
                dispatchNotes,
                MaximumDispatchNotesLength,
                nameof(dispatchNotes),
                "Dispatch notes");

        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        DispatchNotes = normalizedDispatchNotes;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public WorkOrderScheduleTechnician AssignTechnician(
        Guid assignmentId,
        Guid technicianUserId,
        WorkOrderTechnicianRole role,
        DateTimeOffset assignedAtUtc,
        Guid assignedByUserId)
    {
        EnsureScheduled();

        ValidateId(
            assignmentId,
            nameof(assignmentId),
            "Schedule technician assignment ID");

        ValidateId(
            technicianUserId,
            nameof(technicianUserId),
            "Technician user ID");

        if (_technicians.Any(
                technician =>
                    technician.Id == assignmentId))
        {
            throw new InvalidOperationException(
                "A technician assignment with this ID already exists.");
        }

        if (_technicians.Any(
                technician =>
                    technician.TechnicianUserId == technicianUserId))
        {
            throw new InvalidOperationException(
                "The technician is already assigned to this schedule.");
        }

        EnsurePrimaryRoleAvailable(
            role,
            technicianUserId);

        ValidateModification(
            assignedAtUtc,
            assignedByUserId);

        WorkOrderScheduleTechnician assignment =
            WorkOrderScheduleTechnician.Create(
                assignmentId,
                Id,
                technicianUserId,
                role,
                assignedAtUtc,
                assignedByUserId);

        _technicians.Add(assignment);

        MarkModified(
            assignedAtUtc,
            assignedByUserId);

        return assignment;
    }

    public void ChangeTechnicianRole(
        Guid technicianUserId,
        WorkOrderTechnicianRole role,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureScheduled();

        WorkOrderScheduleTechnician assignment =
            GetTechnicianAssignment(
                technicianUserId);

        EnsurePrimaryRoleAvailable(
            role,
            technicianUserId);

        assignment.ChangeRole(
            role,
            modifiedAtUtc,
            modifiedByUserId);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void RemoveTechnician(
        Guid technicianUserId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureScheduled();

        WorkOrderScheduleTechnician assignment =
            GetTechnicianAssignment(
                technicianUserId);

        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        _technicians.Remove(assignment);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void Start(
        DateTimeOffset startedAtUtc,
        Guid startedByUserId)
    {
        EnsureScheduled();

        if (_technicians.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one technician must be assigned before the schedule can be started.");
        }

        ValidateModification(
            startedAtUtc,
            startedByUserId);

        Status = WorkOrderScheduleStatus.InProgress;
        StartedAtUtc = startedAtUtc;
        StartedByUserId = startedByUserId;

        MarkModified(
            startedAtUtc,
            startedByUserId);
    }

    public void Complete(
        DateTimeOffset completedAtUtc,
        Guid completedByUserId)
    {
        if (!IsInProgress)
        {
            throw new InvalidOperationException(
                "Only an in-progress schedule can be completed.");
        }

        ValidateModification(
            completedAtUtc,
            completedByUserId);

        if (StartedAtUtc.HasValue &&
            completedAtUtc < StartedAtUtc.Value)
        {
            throw new ArgumentException(
                "A schedule cannot be completed before it was started.",
                nameof(completedAtUtc));
        }

        Status = WorkOrderScheduleStatus.Completed;
        CompletedAtUtc = completedAtUtc;
        CompletedByUserId = completedByUserId;

        MarkModified(
            completedAtUtc,
            completedByUserId);
    }

    public void Cancel(
        string reason,
        DateTimeOffset cancelledAtUtc,
        Guid cancelledByUserId)
    {
        EnsureActive();

        string normalizedReason =
            NormalizeRequired(
                reason,
                MaximumCancellationReasonLength,
                nameof(reason),
                "Cancellation reason");

        ValidateModification(
            cancelledAtUtc,
            cancelledByUserId);

        if (StartedAtUtc.HasValue &&
            cancelledAtUtc < StartedAtUtc.Value)
        {
            throw new ArgumentException(
                "A schedule cannot be cancelled before it was started.",
                nameof(cancelledAtUtc));
        }

        Status = WorkOrderScheduleStatus.Cancelled;
        CancellationReason = normalizedReason;
        CancelledAtUtc = cancelledAtUtc;
        CancelledByUserId = cancelledByUserId;

        MarkModified(
            cancelledAtUtc,
            cancelledByUserId);
    }

    private WorkOrderScheduleTechnician GetTechnicianAssignment(
        Guid technicianUserId)
    {
        ValidateId(
            technicianUserId,
            nameof(technicianUserId),
            "Technician user ID");

        return _technicians.SingleOrDefault(
                   technician =>
                       technician.TechnicianUserId == technicianUserId)
               ?? throw new InvalidOperationException(
                   "The technician is not assigned to this schedule.");
    }

    private void EnsurePrimaryRoleAvailable(
        WorkOrderTechnicianRole requestedRole,
        Guid technicianUserId)
    {
        if (!Enum.IsDefined(requestedRole))
        {
            throw new ArgumentOutOfRangeException(
                nameof(requestedRole),
                requestedRole,
                "Work order technician role is invalid.");
        }

        if (requestedRole != WorkOrderTechnicianRole.Primary)
        {
            return;
        }

        bool anotherPrimaryExists =
            _technicians.Any(
                technician =>
                    technician.Role == WorkOrderTechnicianRole.Primary &&
                    technician.TechnicianUserId != technicianUserId);

        if (anotherPrimaryExists)
        {
            throw new InvalidOperationException(
                "Only one primary technician can be assigned to a schedule.");
        }
    }

    private void EnsureScheduled()
    {
        if (!IsScheduled)
        {
            throw new InvalidOperationException(
                "Only a scheduled appointment can be changed.");
        }
    }

    private void EnsureActive()
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException(
                "A completed schedule cannot be changed.");
        }

        if (IsCancelled)
        {
            throw new InvalidOperationException(
                "A cancelled schedule cannot be changed.");
        }
    }

    private void ValidateModification(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateId(
            modifiedByUserId,
            nameof(modifiedByUserId),
            "Modified by user ID");

        if (modifiedAtUtc < CreatedAtUtc)
        {
            throw new ArgumentException(
                "Schedule activity cannot occur before the schedule was created.",
                nameof(modifiedAtUtc));
        }
    }

    private static void ValidateScheduleWindow(
        DateTimeOffset scheduledStartUtc,
        DateTimeOffset scheduledEndUtc)
    {
        if (scheduledEndUtc <= scheduledStartUtc)
        {
            throw new ArgumentException(
                "Scheduled end must occur after scheduled start.",
                nameof(scheduledEndUtc));
        }
    }

    private static void ValidateArrivalWindow(
        DateTimeOffset arrivalWindowStartUtc,
        DateTimeOffset arrivalWindowEndUtc,
        DateTimeOffset scheduledStartUtc,
        DateTimeOffset scheduledEndUtc)
    {
        if (arrivalWindowEndUtc <= arrivalWindowStartUtc)
        {
            throw new ArgumentException(
                "Arrival-window end must occur after arrival-window start.",
                nameof(arrivalWindowEndUtc));
        }

        if (arrivalWindowStartUtc < scheduledStartUtc ||
            arrivalWindowEndUtc > scheduledEndUtc)
        {
            throw new ArgumentException(
                "The arrival window must fall within the scheduled appointment.",
                nameof(arrivalWindowStartUtc));
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

    private static string NormalizeRequired(
        string value,
        int maximumLength,
        string parameterName,
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"{displayName} is required.",
                parameterName);
        }

        string normalizedValue =
            value.Trim();

        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{displayName} cannot exceed {maximumLength} characters.",
                parameterName);
        }

        return normalizedValue;
    }

    private static string? NormalizeOptional(
        string? value,
        int maximumLength,
        string parameterName,
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        string normalizedValue =
            value.Trim();

        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{displayName} cannot exceed {maximumLength} characters.",
                parameterName);
        }

        return normalizedValue;
    }
}
