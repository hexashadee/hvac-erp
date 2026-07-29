using HvacERP.Domain.Common.Base;

namespace HvacERP.Domain.WorkOrders;

public sealed class WorkOrder : AggregateRoot
{
    private readonly List<WorkOrderLineItem> _lineItems = [];

    private readonly List<WorkOrderLaborEntry> _laborEntries = [];

    public IReadOnlyCollection<WorkOrderLineItem> LineItems =>
        _lineItems.AsReadOnly();

    public decimal LineItemSubtotal =>
        decimal.Round(
            _lineItems.Sum(lineItem => lineItem.LineTotal),
            2,
            MidpointRounding.AwayFromZero);

    public decimal TaxableLineItemSubtotal =>
        decimal.Round(
            _lineItems
                .Where(lineItem => lineItem.IsTaxable)
                .Sum(lineItem => lineItem.LineTotal),
            2,
            MidpointRounding.AwayFromZero);

    public IReadOnlyCollection<WorkOrderLaborEntry> LaborEntries =>
        _laborEntries.AsReadOnly();

    public decimal TotalLaborHours =>
        decimal.Round(
            _laborEntries.Sum(
                laborEntry => laborEntry.DurationHours),
            2,
            MidpointRounding.AwayFromZero);

    public decimal BillableLaborHours =>
        decimal.Round(
            _laborEntries
                .Where(laborEntry => laborEntry.IsBillable)
                .Sum(laborEntry => laborEntry.DurationHours),
            2,
            MidpointRounding.AwayFromZero);

    public decimal NonBillableLaborHours =>
        decimal.Round(
            _laborEntries
                .Where(laborEntry => !laborEntry.IsBillable)
                .Sum(laborEntry => laborEntry.DurationHours),
            2,
            MidpointRounding.AwayFromZero);

    public decimal LaborSubtotal =>
        decimal.Round(
            _laborEntries.Sum(
                laborEntry => laborEntry.BillableAmount),
            2,
            MidpointRounding.AwayFromZero);

    public decimal WorkOrderSubtotal =>
        decimal.Round(
            LineItemSubtotal + LaborSubtotal,
            2,
            MidpointRounding.AwayFromZero);

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

    public WorkOrderLineItem AddLineItem(
        Guid lineItemId,
        WorkOrderLineItemType type,
        string? itemNumber,
        string description,
        decimal quantity,
        decimal unitPrice,
        bool isTaxable,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        ValidateId(
            lineItemId,
            nameof(lineItemId),
            "Line item ID");

        if (_lineItems.Any(
                lineItem =>
                    lineItem.Id == lineItemId))
        {
            throw new InvalidOperationException(
                "A work order line item with the same ID already exists.");
        }

        ValidateLineItemType(type);

        string? normalizedItemNumber =
            NormalizeOptionalValue(itemNumber);

        string normalizedDescription =
            NormalizeRequiredValue(description);

        ValidateOptionalLength(
            normalizedItemNumber,
            100,
            nameof(itemNumber),
            "Item number");

        ValidateRequiredLength(
            normalizedDescription,
            500,
            nameof(description),
            "Line item description");

        ValidateLineItemQuantity(
            quantity);

        ValidateLineItemUnitPrice(
            type,
            unitPrice);

        WorkOrderLineItem lineItem =
            new(
                lineItemId,
                type,
                normalizedItemNumber,
                normalizedDescription,
                quantity,
                unitPrice,
                isTaxable);

        _lineItems.Add(lineItem);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);

        return lineItem;
    }

    public void UpdateLineItem(
        Guid lineItemId,
        WorkOrderLineItemType type,
        string? itemNumber,
        string description,
        decimal quantity,
        decimal unitPrice,
        bool isTaxable,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        WorkOrderLineItem lineItem =
            GetLineItem(lineItemId);

        ValidateLineItemType(type);

        string? normalizedItemNumber =
            NormalizeOptionalValue(itemNumber);

        string normalizedDescription =
            NormalizeRequiredValue(description);

        ValidateOptionalLength(
            normalizedItemNumber,
            100,
            nameof(itemNumber),
            "Item number");

        ValidateRequiredLength(
            normalizedDescription,
            500,
            nameof(description),
            "Line item description");

        ValidateLineItemQuantity(
            quantity);

        ValidateLineItemUnitPrice(
            type,
            unitPrice);

        lineItem.Update(
            type,
            normalizedItemNumber,
            normalizedDescription,
            quantity,
            unitPrice,
            isTaxable);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void RemoveLineItem(
        Guid lineItemId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        WorkOrderLineItem lineItem =
            GetLineItem(lineItemId);

        _lineItems.Remove(lineItem);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public WorkOrderLaborEntry AddLaborEntry(
        Guid laborEntryId,
        Guid technicianUserId,
        WorkOrderLaborType type,
        DateTimeOffset startedAtUtc,
        DateTimeOffset endedAtUtc,
        bool isBillable,
        decimal hourlyRate,
        string? notes,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        ValidateId(
            laborEntryId,
            nameof(laborEntryId),
            "Labor entry ID");

        if (_laborEntries.Any(
                laborEntry =>
                    laborEntry.Id == laborEntryId))
        {
            throw new InvalidOperationException(
                "A work order labor entry with the same ID already exists.");
        }

        ValidateId(
            technicianUserId,
            nameof(technicianUserId),
            "Technician user ID");

        ValidateLaborType(type);

        ValidateLaborTimeRange(
            startedAtUtc,
            endedAtUtc);

        ValidateHourlyRate(
            hourlyRate);

        string? normalizedNotes =
            NormalizeOptionalValue(notes);

        ValidateOptionalLength(
            normalizedNotes,
            2000,
            nameof(notes),
            "Labor notes");

        WorkOrderLaborEntry laborEntry =
            new(
                laborEntryId,
                technicianUserId,
                type,
                startedAtUtc,
                endedAtUtc,
                isBillable,
                hourlyRate,
                normalizedNotes);

        _laborEntries.Add(laborEntry);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);

        return laborEntry;
    }

    public void UpdateLaborEntry(
        Guid laborEntryId,
        Guid technicianUserId,
        WorkOrderLaborType type,
        DateTimeOffset startedAtUtc,
        DateTimeOffset endedAtUtc,
        bool isBillable,
        decimal hourlyRate,
        string? notes,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        WorkOrderLaborEntry laborEntry =
            GetLaborEntry(laborEntryId);

        ValidateId(
            technicianUserId,
            nameof(technicianUserId),
            "Technician user ID");

        ValidateLaborType(type);

        ValidateLaborTimeRange(
            startedAtUtc,
            endedAtUtc);

        ValidateHourlyRate(
            hourlyRate);

        string? normalizedNotes =
            NormalizeOptionalValue(notes);

        ValidateOptionalLength(
            normalizedNotes,
            2000,
            nameof(notes),
            "Labor notes");

        laborEntry.Update(
            technicianUserId,
            type,
            startedAtUtc,
            endedAtUtc,
            isBillable,
            hourlyRate,
            normalizedNotes);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void RemoveLaborEntry(
        Guid laborEntryId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotClosed();

        WorkOrderLaborEntry laborEntry =
            GetLaborEntry(laborEntryId);

        _laborEntries.Remove(laborEntry);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
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

    private WorkOrderLaborEntry GetLaborEntry(
        Guid laborEntryId)
    {
        ValidateId(
            laborEntryId,
            nameof(laborEntryId),
            "Labor entry ID");

        WorkOrderLaborEntry? laborEntry =
            _laborEntries.SingleOrDefault(
                existingLaborEntry =>
                    existingLaborEntry.Id == laborEntryId);

        if (laborEntry is null)
        {
            throw new InvalidOperationException(
                "The work order labor entry does not exist.");
        }

        return laborEntry;
    }

    private static void ValidateLaborType(
        WorkOrderLaborType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Work order labor type is invalid.");
        }
    }

    private static void ValidateLaborTimeRange(
        DateTimeOffset startedAtUtc,
        DateTimeOffset endedAtUtc)
    {
        if (endedAtUtc <= startedAtUtc)
        {
            throw new ArgumentException(
                "Labor end time must be after the labor start time.",
                nameof(endedAtUtc));
        }

        if (endedAtUtc - startedAtUtc >
            TimeSpan.FromHours(24))
        {
            throw new ArgumentException(
                "A single labor entry cannot exceed 24 hours.",
                nameof(endedAtUtc));
        }
    }

    private static void ValidateHourlyRate(
        decimal hourlyRate)
    {
        if (hourlyRate < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hourlyRate),
                hourlyRate,
                "Hourly rate cannot be negative.");
        }

        if (hourlyRate > 10_000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hourlyRate),
                hourlyRate,
                "Hourly rate cannot exceed 10,000.");
        }
    }

    private WorkOrderLineItem GetLineItem(
        Guid lineItemId)
    {
        ValidateId(
            lineItemId,
            nameof(lineItemId),
            "Line item ID");

        WorkOrderLineItem? lineItem =
            _lineItems.SingleOrDefault(
                existingLineItem =>
                    existingLineItem.Id == lineItemId);

        if (lineItem is null)
        {
            throw new InvalidOperationException(
                "The work order line item does not exist.");
        }

        return lineItem;
    }

    private static void ValidateLineItemType(
        WorkOrderLineItemType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Work order line item type is invalid.");
        }
    }

    private static void ValidateLineItemQuantity(
        decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                "Line item quantity must be greater than zero.");
        }

        if (quantity > 1_000_000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                "Line item quantity cannot exceed 1,000,000.");
        }
    }

    private static void ValidateLineItemUnitPrice(
        WorkOrderLineItemType type,
        decimal unitPrice)
    {
        if (type == WorkOrderLineItemType.Discount)
        {
            if (unitPrice >= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(unitPrice),
                    unitPrice,
                    "A discount line item must have a negative unit price.");
            }
        }
        else if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(unitPrice),
                unitPrice,
                "Only discount line items may have a negative unit price.");
        }

        if (unitPrice < -1_000_000 ||
            unitPrice > 1_000_000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(unitPrice),
                unitPrice,
                "Line item unit price must be between -1,000,000 and 1,000,000.");
        }
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
