using HvacERP.Domain.Common.Base;

namespace HvacERP.Domain.WorkOrders;

public sealed class WorkOrderChecklist : AggregateRoot
{
    public const int MaximumNameLength = 200;

    public const int MaximumDescriptionLength = 1_000;

    public const int MaximumDeletionReasonLength = 500;

    private readonly List<WorkOrderChecklistItem> _items = [];

    public Guid WorkOrderId { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public WorkOrderChecklistStatus Status { get; private set; }

    public IReadOnlyCollection<WorkOrderChecklistItem> Items =>
        _items
            .OrderBy(item => item.DisplayOrder)
            .ThenBy(item => item.CreatedAtUtc)
            .ToArray();

    public int TotalItemCount =>
        _items.Count;

    public int CompletedItemCount =>
        _items.Count(item => item.IsCompleted);

    public int FailedItemCount =>
        _items.Count(
            item => item.Result == ChecklistItemResult.Failed);

    public bool HasFailures =>
        FailedItemCount > 0;

    public bool IsComplete =>
        Status == WorkOrderChecklistStatus.Completed;

    public bool IsDeleted =>
        Status == WorkOrderChecklistStatus.Deleted;

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public Guid? CompletedByUserId { get; private set; }

    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public Guid? DeletedByUserId { get; private set; }

    public string? DeletionReason { get; private set; }

    private WorkOrderChecklist(
        Guid id,
        Guid workOrderId,
        string name,
        string? description,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(
            id,
            createdAtUtc,
            createdByUserId)
    {
        WorkOrderId = workOrderId;
        Name = name;
        Description = description;
        Status = WorkOrderChecklistStatus.InProgress;
    }

    public static WorkOrderChecklist Create(
        Guid id,
        Guid workOrderId,
        string name,
        string? description,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        ValidateId(
            id,
            nameof(id),
            "Work order checklist ID");

        ValidateId(
            workOrderId,
            nameof(workOrderId),
            "Work order ID");

        string normalizedName =
            NormalizeRequired(
                name,
                MaximumNameLength,
                nameof(name),
                "Checklist name");

        string? normalizedDescription =
            NormalizeOptional(
                description,
                MaximumDescriptionLength,
                nameof(description),
                "Checklist description");

        ValidateId(
            createdByUserId,
            nameof(createdByUserId),
            "Created by user ID");

        return new WorkOrderChecklist(
            id,
            workOrderId,
            normalizedName,
            normalizedDescription,
            createdAtUtc,
            createdByUserId);
    }

    public WorkOrderChecklistItem AddItem(
        Guid itemId,
        string category,
        string description,
        ChecklistItemResponseType responseType,
        bool isRequired,
        bool requiresPhoto,
        int displayOrder,
        string? unit,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        EnsureEditable();

        if (_items.Any(item => item.Id == itemId))
        {
            throw new InvalidOperationException(
                "A checklist item with this ID already exists.");
        }

        EnsureActivityNotBeforeChecklistCreation(
            createdAtUtc,
            nameof(createdAtUtc));

        WorkOrderChecklistItem item =
            WorkOrderChecklistItem.Create(
                itemId,
                Id,
                category,
                description,
                responseType,
                isRequired,
                requiresPhoto,
                displayOrder,
                unit,
                createdAtUtc,
                createdByUserId);

        _items.Add(item);

        MarkModified(
            createdAtUtc,
            createdByUserId);

        return item;
    }

    public void UpdateItemDefinition(
        Guid itemId,
        string category,
        string description,
        bool isRequired,
        bool requiresPhoto,
        int displayOrder,
        string? unit,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureEditable();

        WorkOrderChecklistItem item =
            GetItem(itemId);

        item.UpdateDefinition(
            category,
            description,
            isRequired,
            requiresPhoto,
            displayOrder,
            unit,
            modifiedAtUtc,
            modifiedByUserId);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void RecordPassFail(
        Guid itemId,
        ChecklistItemResult result,
        string? notes,
        Guid? photoDocumentId,
        DateTimeOffset completedAtUtc,
        Guid completedByUserId)
    {
        EnsureEditable();

        WorkOrderChecklistItem item =
            GetItem(itemId);

        item.RecordPassFail(
            result,
            notes,
            photoDocumentId,
            completedAtUtc,
            completedByUserId);

        MarkModified(
            completedAtUtc,
            completedByUserId);
    }

    public void RecordNumericReading(
        Guid itemId,
        decimal numericReading,
        ChecklistItemResult result,
        string? notes,
        Guid? photoDocumentId,
        DateTimeOffset completedAtUtc,
        Guid completedByUserId)
    {
        EnsureEditable();

        WorkOrderChecklistItem item =
            GetItem(itemId);

        item.RecordNumericReading(
            numericReading,
            result,
            notes,
            photoDocumentId,
            completedAtUtc,
            completedByUserId);

        MarkModified(
            completedAtUtc,
            completedByUserId);
    }

    public void RecordText(
        Guid itemId,
        string responseText,
        string? notes,
        Guid? photoDocumentId,
        DateTimeOffset completedAtUtc,
        Guid completedByUserId)
    {
        EnsureEditable();

        WorkOrderChecklistItem item =
            GetItem(itemId);

        item.RecordText(
            responseText,
            notes,
            photoDocumentId,
            completedAtUtc,
            completedByUserId);

        MarkModified(
            completedAtUtc,
            completedByUserId);
    }

    public void RecordConfirmation(
        Guid itemId,
        bool confirmed,
        string? notes,
        Guid? photoDocumentId,
        DateTimeOffset completedAtUtc,
        Guid completedByUserId)
    {
        EnsureEditable();

        WorkOrderChecklistItem item =
            GetItem(itemId);

        item.RecordConfirmation(
            confirmed,
            notes,
            photoDocumentId,
            completedAtUtc,
            completedByUserId);

        MarkModified(
            completedAtUtc,
            completedByUserId);
    }

    public void ResetItem(
        Guid itemId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureEditable();

        WorkOrderChecklistItem item =
            GetItem(itemId);

        item.Reset(
            modifiedAtUtc,
            modifiedByUserId);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void RemoveItem(
        Guid itemId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureEditable();

        WorkOrderChecklistItem item =
            GetItem(itemId);

        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        _items.Remove(item);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void UpdateDetails(
        string name,
        string? description,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureEditable();

        string normalizedName =
            NormalizeRequired(
                name,
                MaximumNameLength,
                nameof(name),
                "Checklist name");

        string? normalizedDescription =
            NormalizeOptional(
                description,
                MaximumDescriptionLength,
                nameof(description),
                "Checklist description");

        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        Name = normalizedName;
        Description = normalizedDescription;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void Complete(
        DateTimeOffset completedAtUtc,
        Guid completedByUserId)
    {
        EnsureEditable();

        if (_items.Count == 0)
        {
            throw new InvalidOperationException(
                "A checklist must contain at least one item before it can be completed.");
        }

        WorkOrderChecklistItem[] incompleteRequiredItems =
            _items
                .Where(
                    item =>
                        item.IsRequired &&
                        !item.IsCompleted)
                .ToArray();

        if (incompleteRequiredItems.Length > 0)
        {
            throw new InvalidOperationException(
                "All required checklist items must be completed.");
        }

        ValidateModification(
            completedAtUtc,
            completedByUserId);

        Status = WorkOrderChecklistStatus.Completed;
        CompletedAtUtc = completedAtUtc;
        CompletedByUserId = completedByUserId;

        MarkModified(
            completedAtUtc,
            completedByUserId);
    }

    public void Reopen(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotDeleted();

        if (Status != WorkOrderChecklistStatus.Completed)
        {
            throw new InvalidOperationException(
                "Only a completed checklist can be reopened.");
        }

        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        Status = WorkOrderChecklistStatus.InProgress;
        CompletedAtUtc = null;
        CompletedByUserId = null;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void Delete(
        string deletionReason,
        DateTimeOffset deletedAtUtc,
        Guid deletedByUserId)
    {
        EnsureNotDeleted();

        string normalizedDeletionReason =
            NormalizeRequired(
                deletionReason,
                MaximumDeletionReasonLength,
                nameof(deletionReason),
                "Deletion reason");

        ValidateModification(
            deletedAtUtc,
            deletedByUserId);

        Status = WorkOrderChecklistStatus.Deleted;
        DeletionReason = normalizedDeletionReason;
        DeletedAtUtc = deletedAtUtc;
        DeletedByUserId = deletedByUserId;

        MarkModified(
            deletedAtUtc,
            deletedByUserId);
    }

    private WorkOrderChecklistItem GetItem(
        Guid itemId)
    {
        ValidateId(
            itemId,
            nameof(itemId),
            "Checklist item ID");

        return _items.SingleOrDefault(
                   item => item.Id == itemId)
               ?? throw new InvalidOperationException(
                   "The checklist item was not found.");
    }

    private void EnsureEditable()
    {
        EnsureNotDeleted();

        if (Status == WorkOrderChecklistStatus.Completed)
        {
            throw new InvalidOperationException(
                "A completed checklist cannot be changed.");
        }
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "A deleted checklist cannot be changed.");
        }
    }

    private void EnsureActivityNotBeforeChecklistCreation(
        DateTimeOffset activityAtUtc,
        string parameterName)
    {
        if (activityAtUtc < CreatedAtUtc)
        {
            throw new ArgumentException(
                "Checklist activity cannot occur before the checklist was created.",
                parameterName);
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

        EnsureActivityNotBeforeChecklistCreation(
            modifiedAtUtc,
            nameof(modifiedAtUtc));
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
