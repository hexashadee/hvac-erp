using HvacERP.Domain.Common.Base;

namespace HvacERP.Domain.WorkOrders;

public sealed class WorkOrder : AggregateRoot
{
    public Guid CompanyId { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid CustomerLocationId { get; private set; }

    public Guid? EquipmentId { get; private set; }

    public string WorkOrderNumber { get; private set; }

    public WorkOrderType Type { get; private set; }

    public WorkOrderPriority Priority { get; private set; }

    public WorkOrderStatus Status { get; private set; }

    public string Summary { get; private set; }

    public string? Description { get; private set; }

    public DateTimeOffset? RequestedForUtc { get; private set; }

    public DateTimeOffset? ScheduledStartUtc { get; private set; }

    public DateTimeOffset? ScheduledEndUtc { get; private set; }

    public Guid? AssignedTechnicianUserId { get; private set; }

    public DateTimeOffset? DispatchedAtUtc { get; private set; }

    public DateTimeOffset? StartedAtUtc { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public DateTimeOffset? CancelledAtUtc { get; private set; }

    public string? CancellationReason { get; private set; }

    private WorkOrder(
        Guid id,
        Guid companyId,
        Guid customerId,
        Guid customerLocationId,
        Guid? equipmentId,
        string workOrderNumber,
        WorkOrderType type,
        WorkOrderPriority priority,
        string summary,
        string? description,
        DateTimeOffset? requestedForUtc,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(
            id,
            createdAtUtc,
            createdByUserId)
    {
        CompanyId = companyId;
        CustomerId = customerId;
        CustomerLocationId = customerLocationId;
        EquipmentId = equipmentId;
        WorkOrderNumber = workOrderNumber;
        Type = type;
        Priority = priority;
        Status = WorkOrderStatus.Draft;
        Summary = summary;
        Description = description;
        RequestedForUtc = requestedForUtc;
    }

    public static WorkOrder Create(
        Guid id,
        Guid companyId,
        Guid customerId,
        Guid customerLocationId,
        Guid? equipmentId,
        string workOrderNumber,
        WorkOrderType type,
        WorkOrderPriority priority,
        string summary,
        string? description,
        DateTimeOffset? requestedForUtc,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        ValidateId(
            id,
            nameof(id),
            "Work order ID");

        ValidateId(
            companyId,
            nameof(companyId),
            "Company ID");

        ValidateId(
            customerId,
            nameof(customerId),
            "Customer ID");

        ValidateId(
            customerLocationId,
            nameof(customerLocationId),
            "Customer location ID");

        ValidateOptionalId(
            equipmentId,
            nameof(equipmentId),
            "Equipment ID");

        string normalizedWorkOrderNumber =
            NormalizeRequiredValue(workOrderNumber)
                .ToUpperInvariant();

        string normalizedSummary =
            NormalizeRequiredValue(summary);

        string? normalizedDescription =
            NormalizeOptionalValue(description);

        ValidateWorkOrderNumber(
            normalizedWorkOrderNumber);

        ValidateType(type);
        ValidatePriority(priority);

        ValidateRequiredLength(
            normalizedSummary,
            200,
            nameof(summary),
            "Summary");

        ValidateOptionalLength(
            normalizedDescription,
            4000,
            nameof(description),
            "Description");

        return new WorkOrder(
            id,
            companyId,
            customerId,
            customerLocationId,
            equipmentId,
            normalizedWorkOrderNumber,
            type,
            priority,
            normalizedSummary,
            normalizedDescription,
            requestedForUtc,
            createdAtUtc,
            createdByUserId);
    }

    public void UpdateDetails(
        WorkOrderType type,
        WorkOrderPriority priority,
        string summary,
        string? description,
        DateTimeOffset? requestedForUtc,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        ValidateType(type);
        ValidatePriority(priority);

        string normalizedSummary =
            NormalizeRequiredValue(summary);

        string? normalizedDescription =
            NormalizeOptionalValue(description);

        ValidateRequiredLength(
            normalizedSummary,
            200,
            nameof(summary),
            "Summary");

        ValidateOptionalLength(
            normalizedDescription,
            4000,
            nameof(description),
            "Description");

        Type = type;
        Priority = priority;
        Summary = normalizedSummary;
        Description = normalizedDescription;
        RequestedForUtc = requestedForUtc;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void ChangeLocation(
        Guid customerLocationId,
        Guid? equipmentId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        ValidateId(
            customerLocationId,
            nameof(customerLocationId),
            "Customer location ID");

        ValidateOptionalId(
            equipmentId,
            nameof(equipmentId),
            "Equipment ID");

        if (CustomerLocationId == customerLocationId &&
            EquipmentId == equipmentId)
        {
            return;
        }

        CustomerLocationId = customerLocationId;
        EquipmentId = equipmentId;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void SetEquipment(
        Guid? equipmentId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        ValidateOptionalId(
            equipmentId,
            nameof(equipmentId),
            "Equipment ID");

        if (EquipmentId == equipmentId)
        {
            return;
        }

        EquipmentId = equipmentId;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void Open(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureStatus(
            WorkOrderStatus.Draft,
            "Only a draft work order can be opened.");

        Status = WorkOrderStatus.Open;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void Schedule(
        DateTimeOffset scheduledStartUtc,
        DateTimeOffset scheduledEndUtc,
        Guid? assignedTechnicianUserId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        if (Status is not WorkOrderStatus.Open and
            not WorkOrderStatus.Scheduled and
            not WorkOrderStatus.OnHold)
        {
            throw new InvalidOperationException(
                "Only an open, scheduled, or on-hold work order can be scheduled.");
        }

        if (scheduledEndUtc <= scheduledStartUtc)
        {
            throw new ArgumentException(
                "Scheduled end time must be after the scheduled start time.",
                nameof(scheduledEndUtc));
        }

        ValidateOptionalId(
            assignedTechnicianUserId,
            nameof(assignedTechnicianUserId),
            "Assigned technician user ID");

        ScheduledStartUtc = scheduledStartUtc;
        ScheduledEndUtc = scheduledEndUtc;
        AssignedTechnicianUserId =
            assignedTechnicianUserId;

        Status = WorkOrderStatus.Scheduled;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void AssignTechnician(
        Guid assignedTechnicianUserId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        ValidateId(
            assignedTechnicianUserId,
            nameof(assignedTechnicianUserId),
            "Assigned technician user ID");

        if (AssignedTechnicianUserId ==
            assignedTechnicianUserId)
        {
            return;
        }

        AssignedTechnicianUserId =
            assignedTechnicianUserId;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void UnassignTechnician(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        if (AssignedTechnicianUserId is null)
        {
            return;
        }

        AssignedTechnicianUserId = null;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void Dispatch(
        DateTimeOffset dispatchedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureStatus(
            WorkOrderStatus.Scheduled,
            "Only a scheduled work order can be dispatched.");

        if (AssignedTechnicianUserId is null)
        {
            throw new InvalidOperationException(
                "A technician must be assigned before dispatch.");
        }

        Status = WorkOrderStatus.Dispatched;
        DispatchedAtUtc = dispatchedAtUtc;

        MarkModified(
            dispatchedAtUtc,
            modifiedByUserId);
    }

    public void Start(
        DateTimeOffset startedAtUtc,
        Guid modifiedByUserId)
    {
        if (Status is not WorkOrderStatus.Scheduled and
            not WorkOrderStatus.Dispatched and
            not WorkOrderStatus.OnHold)
        {
            throw new InvalidOperationException(
                "Only a scheduled, dispatched, or on-hold work order can be started.");
        }

        if (AssignedTechnicianUserId is null)
        {
            throw new InvalidOperationException(
                "A technician must be assigned before work can begin.");
        }

        Status = WorkOrderStatus.InProgress;
        StartedAtUtc ??= startedAtUtc;

        MarkModified(
            startedAtUtc,
            modifiedByUserId);
    }

    public void PlaceOnHold(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        if (Status is not WorkOrderStatus.Scheduled and
            not WorkOrderStatus.Dispatched and
            not WorkOrderStatus.InProgress)
        {
            throw new InvalidOperationException(
                "Only a scheduled, dispatched, or in-progress work order can be placed on hold.");
        }

        Status = WorkOrderStatus.OnHold;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void Complete(
        DateTimeOffset completedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureStatus(
            WorkOrderStatus.InProgress,
            "Only an in-progress work order can be completed.");

        Status = WorkOrderStatus.Completed;
        CompletedAtUtc = completedAtUtc;

        MarkModified(
            completedAtUtc,
            modifiedByUserId);
    }

    public void Cancel(
        string cancellationReason,
        DateTimeOffset cancelledAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        string normalizedCancellationReason =
            NormalizeRequiredValue(
                cancellationReason);

        ValidateRequiredLength(
            normalizedCancellationReason,
            1000,
            nameof(cancellationReason),
            "Cancellation reason");

        Status = WorkOrderStatus.Cancelled;
        CancelledAtUtc = cancelledAtUtc;
        CancellationReason =
            normalizedCancellationReason;

        MarkModified(
            cancelledAtUtc,
            modifiedByUserId);
    }

    private void EnsureNotClosed()
    {
        if (Status is WorkOrderStatus.Completed or
            WorkOrderStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "A completed or cancelled work order cannot be changed.");
        }
    }

    private void EnsureStatus(
        WorkOrderStatus requiredStatus,
        string message)
    {
        if (Status != requiredStatus)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static string NormalizeRequiredValue(
        string value)
    {
        return value?.Trim() ?? string.Empty;
    }

    private static string? NormalizeOptionalValue(
        string? value)
    {
        string? normalizedValue =
            value?.Trim();

        return string.IsNullOrWhiteSpace(
            normalizedValue)
            ? null
            : normalizedValue;
    }

    private static void ValidateId(
        Guid id,
        string parameterName,
        string displayName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                $"{displayName} cannot be empty.",
                parameterName);
        }
    }

    private static void ValidateOptionalId(
        Guid? id,
        string parameterName,
        string displayName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                $"{displayName} cannot be empty.",
                parameterName);
        }
    }

    private static void ValidateWorkOrderNumber(
        string workOrderNumber)
    {
        ValidateRequiredLength(
            workOrderNumber,
            50,
            nameof(workOrderNumber),
            "Work order number");

        if (workOrderNumber.Any(
                character =>
                    !char.IsLetterOrDigit(character) &&
                    character != '-'))
        {
            throw new ArgumentException(
                "Work order number may contain only letters, numbers, and hyphens.",
                nameof(workOrderNumber));
        }
    }

    private static void ValidateType(
        WorkOrderType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Work order type is invalid.");
        }
    }

    private static void ValidatePriority(
        WorkOrderPriority priority)
    {
        if (!Enum.IsDefined(priority))
        {
            throw new ArgumentOutOfRangeException(
                nameof(priority),
                priority,
                "Work order priority is invalid.");
        }
    }

    private static void ValidateRequiredLength(
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

        if (value.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{displayName} cannot exceed {maximumLength} characters.",
                parameterName);
        }
    }

    private static void ValidateOptionalLength(
        string? value,
        int maximumLength,
        string parameterName,
        string displayName)
    {
        if (value?.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{displayName} cannot exceed {maximumLength} characters.",
                parameterName);
        }
    }
}
