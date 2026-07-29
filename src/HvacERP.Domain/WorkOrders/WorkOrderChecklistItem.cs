namespace HvacERP.Domain.WorkOrders;

public sealed class WorkOrderChecklistItem
{
    public const int MaximumCategoryLength = 100;

    public const int MaximumDescriptionLength = 500;

    public const int MaximumUnitLength = 50;

    public const int MaximumResponseTextLength = 2_000;

    public const int MaximumNotesLength = 2_000;

    public const int MinimumDisplayOrder = 0;

    public const int MaximumDisplayOrder = 10_000;

    public Guid Id { get; private set; }

    public Guid WorkOrderChecklistId { get; private set; }

    public string Category { get; private set; }

    public string Description { get; private set; }

    public ChecklistItemResponseType ResponseType { get; private set; }

    public bool IsRequired { get; private set; }

    public bool RequiresPhoto { get; private set; }

    public int DisplayOrder { get; private set; }

    public ChecklistItemResult Result { get; private set; }

    public decimal? NumericReading { get; private set; }

    public string? Unit { get; private set; }

    public string? ResponseText { get; private set; }

    public string? Notes { get; private set; }

    public Guid? PhotoDocumentId { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public Guid? CompletedByUserId { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public DateTimeOffset? LastModifiedAtUtc { get; private set; }

    public Guid? LastModifiedByUserId { get; private set; }

    public bool IsCompleted =>
        Result != ChecklistItemResult.NotCompleted;

    private WorkOrderChecklistItem(
        Guid id,
        Guid workOrderChecklistId,
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
        Id = id;
        WorkOrderChecklistId = workOrderChecklistId;
        Category = category;
        Description = description;
        ResponseType = responseType;
        IsRequired = isRequired;
        RequiresPhoto = requiresPhoto;
        DisplayOrder = displayOrder;
        Unit = unit;
        Result = ChecklistItemResult.NotCompleted;
        CreatedAtUtc = createdAtUtc;
        CreatedByUserId = createdByUserId;
    }

    internal static WorkOrderChecklistItem Create(
        Guid id,
        Guid workOrderChecklistId,
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
        ValidateId(
            id,
            nameof(id),
            "Checklist item ID");

        ValidateId(
            workOrderChecklistId,
            nameof(workOrderChecklistId),
            "Work order checklist ID");

        string normalizedCategory =
            NormalizeRequired(
                category,
                MaximumCategoryLength,
                nameof(category),
                "Category");

        string normalizedDescription =
            NormalizeRequired(
                description,
                MaximumDescriptionLength,
                nameof(description),
                "Description");

        ValidateResponseType(responseType);

        ValidateDisplayOrder(displayOrder);

        string? normalizedUnit =
            NormalizeOptional(
                unit,
                MaximumUnitLength,
                nameof(unit),
                "Unit");

        if (responseType != ChecklistItemResponseType.NumericReading &&
            normalizedUnit is not null)
        {
            throw new ArgumentException(
                "A unit can only be assigned to a numeric-reading item.",
                nameof(unit));
        }

        ValidateId(
            createdByUserId,
            nameof(createdByUserId),
            "Created by user ID");

        return new WorkOrderChecklistItem(
            id,
            workOrderChecklistId,
            normalizedCategory,
            normalizedDescription,
            responseType,
            isRequired,
            requiresPhoto,
            displayOrder,
            normalizedUnit,
            createdAtUtc,
            createdByUserId);
    }

    internal void RecordPassFail(
        ChecklistItemResult result,
        string? notes,
        Guid? photoDocumentId,
        DateTimeOffset completedAtUtc,
        Guid completedByUserId)
    {
        EnsureResponseType(
            ChecklistItemResponseType.PassFail);

        if (result != ChecklistItemResult.Passed &&
            result != ChecklistItemResult.Failed &&
            result != ChecklistItemResult.NotApplicable)
        {
            throw new ArgumentOutOfRangeException(
                nameof(result),
                result,
                "A pass/fail item must be passed, failed, or not applicable.");
        }

        ApplyResponse(
            result,
            null,
            null,
            notes,
            photoDocumentId,
            completedAtUtc,
            completedByUserId);
    }

    internal void RecordNumericReading(
        decimal numericReading,
        ChecklistItemResult result,
        string? notes,
        Guid? photoDocumentId,
        DateTimeOffset completedAtUtc,
        Guid completedByUserId)
    {
        EnsureResponseType(
            ChecklistItemResponseType.NumericReading);

        if (result != ChecklistItemResult.Passed &&
            result != ChecklistItemResult.Failed &&
            result != ChecklistItemResult.NotApplicable)
        {
            throw new ArgumentOutOfRangeException(
                nameof(result),
                result,
                "A numeric-reading item must be passed, failed, or not applicable.");
        }

        decimal? storedReading =
            result == ChecklistItemResult.NotApplicable
                ? null
                : numericReading;

        ApplyResponse(
            result,
            storedReading,
            null,
            notes,
            photoDocumentId,
            completedAtUtc,
            completedByUserId);
    }

    internal void RecordText(
        string responseText,
        string? notes,
        Guid? photoDocumentId,
        DateTimeOffset completedAtUtc,
        Guid completedByUserId)
    {
        EnsureResponseType(
            ChecklistItemResponseType.Text);

        string normalizedResponseText =
            NormalizeRequired(
                responseText,
                MaximumResponseTextLength,
                nameof(responseText),
                "Response text");

        ApplyResponse(
            ChecklistItemResult.Passed,
            null,
            normalizedResponseText,
            notes,
            photoDocumentId,
            completedAtUtc,
            completedByUserId);
    }

    internal void RecordConfirmation(
        bool confirmed,
        string? notes,
        Guid? photoDocumentId,
        DateTimeOffset completedAtUtc,
        Guid completedByUserId)
    {
        EnsureResponseType(
            ChecklistItemResponseType.Confirmation);

        ApplyResponse(
            confirmed
                ? ChecklistItemResult.Passed
                : ChecklistItemResult.Failed,
            null,
            null,
            notes,
            photoDocumentId,
            completedAtUtc,
            completedByUserId);
    }

    internal void Reset(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        Result = ChecklistItemResult.NotCompleted;
        NumericReading = null;
        ResponseText = null;
        Notes = null;
        PhotoDocumentId = null;
        CompletedAtUtc = null;
        CompletedByUserId = null;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    internal void UpdateDefinition(
        string category,
        string description,
        bool isRequired,
        bool requiresPhoto,
        int displayOrder,
        string? unit,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException(
                "A completed checklist item definition cannot be changed.");
        }

        string normalizedCategory =
            NormalizeRequired(
                category,
                MaximumCategoryLength,
                nameof(category),
                "Category");

        string normalizedDescription =
            NormalizeRequired(
                description,
                MaximumDescriptionLength,
                nameof(description),
                "Description");

        ValidateDisplayOrder(displayOrder);

        string? normalizedUnit =
            NormalizeOptional(
                unit,
                MaximumUnitLength,
                nameof(unit),
                "Unit");

        if (ResponseType != ChecklistItemResponseType.NumericReading &&
            normalizedUnit is not null)
        {
            throw new ArgumentException(
                "A unit can only be assigned to a numeric-reading item.",
                nameof(unit));
        }

        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        Category = normalizedCategory;
        Description = normalizedDescription;
        IsRequired = isRequired;
        RequiresPhoto = requiresPhoto;
        DisplayOrder = displayOrder;
        Unit = normalizedUnit;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    private void ApplyResponse(
        ChecklistItemResult result,
        decimal? numericReading,
        string? responseText,
        string? notes,
        Guid? photoDocumentId,
        DateTimeOffset completedAtUtc,
        Guid completedByUserId)
    {
        string? normalizedNotes =
            NormalizeOptional(
                notes,
                MaximumNotesLength,
                nameof(notes),
                "Notes");

        if (RequiresPhoto &&
            !photoDocumentId.HasValue)
        {
            throw new InvalidOperationException(
                "This checklist item requires a photo.");
        }

        if (photoDocumentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Photo document ID cannot be empty.",
                nameof(photoDocumentId));
        }

        ValidateModification(
            completedAtUtc,
            completedByUserId);

        Result = result;
        NumericReading = numericReading;
        ResponseText = responseText;
        Notes = normalizedNotes;
        PhotoDocumentId = photoDocumentId;
        CompletedAtUtc = completedAtUtc;
        CompletedByUserId = completedByUserId;

        MarkModified(
            completedAtUtc,
            completedByUserId);
    }

    private void EnsureResponseType(
        ChecklistItemResponseType expectedType)
    {
        if (ResponseType != expectedType)
        {
            throw new InvalidOperationException(
                $"This checklist item requires a {ResponseType} response.");
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
                "Checklist item activity cannot occur before the item was created.",
                nameof(modifiedAtUtc));
        }
    }

    private void MarkModified(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        LastModifiedAtUtc = modifiedAtUtc;
        LastModifiedByUserId = modifiedByUserId;
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

    private static void ValidateResponseType(
        ChecklistItemResponseType responseType)
    {
        if (!Enum.IsDefined(responseType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(responseType),
                responseType,
                "Checklist item response type is invalid.");
        }
    }

    private static void ValidateDisplayOrder(
        int displayOrder)
    {
        if (displayOrder < MinimumDisplayOrder ||
            displayOrder > MaximumDisplayOrder)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                displayOrder,
                $"Display order must be between {MinimumDisplayOrder} and {MaximumDisplayOrder}.");
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
