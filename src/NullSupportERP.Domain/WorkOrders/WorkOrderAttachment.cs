using NullSupportERP.Domain.Common.Base;

namespace NullSupportERP.Domain.WorkOrders;

public sealed class WorkOrderAttachment : AggregateRoot
{
    public const int MaximumCaptionLength = 500;

    public const int MaximumDeletionReasonLength = 500;

    public const int MinimumDisplayOrder = 0;

    public const int MaximumDisplayOrder = 10_000;

    public Guid WorkOrderId { get; private set; }

    public Guid DocumentId { get; private set; }

    public WorkOrderAttachmentType AttachmentType { get; private set; }

    public string? Caption { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsPrimary { get; private set; }

    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public Guid? DeletedByUserId { get; private set; }

    public string? DeletionReason { get; private set; }

    public bool IsDeleted =>
        DeletedAtUtc.HasValue;

    private WorkOrderAttachment(
        Guid id,
        Guid workOrderId,
        Guid documentId,
        WorkOrderAttachmentType attachmentType,
        string? caption,
        int displayOrder,
        bool isPrimary,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(
            id,
            createdAtUtc,
            createdByUserId)
    {
        WorkOrderId = workOrderId;
        DocumentId = documentId;
        AttachmentType = attachmentType;
        Caption = caption;
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
    }

    public static WorkOrderAttachment Create(
        Guid id,
        Guid workOrderId,
        Guid documentId,
        WorkOrderAttachmentType attachmentType,
        string? caption,
        int displayOrder,
        bool isPrimary,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        ValidateId(
            id,
            nameof(id),
            "Work order attachment ID");

        ValidateId(
            workOrderId,
            nameof(workOrderId),
            "Work order ID");

        ValidateId(
            documentId,
            nameof(documentId),
            "Document ID");

        ValidateAttachmentType(attachmentType);

        string? normalizedCaption =
            NormalizeOptional(
                caption,
                MaximumCaptionLength,
                nameof(caption),
                "Caption");

        ValidateDisplayOrder(displayOrder);

        ValidateId(
            createdByUserId,
            nameof(createdByUserId),
            "Created by user ID");

        return new WorkOrderAttachment(
            id,
            workOrderId,
            documentId,
            attachmentType,
            normalizedCaption,
            displayOrder,
            isPrimary,
            createdAtUtc,
            createdByUserId);
    }

    public void UpdateDetails(
        WorkOrderAttachmentType attachmentType,
        string? caption,
        int displayOrder,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotDeleted();

        ValidateAttachmentType(attachmentType);

        string? normalizedCaption =
            NormalizeOptional(
                caption,
                MaximumCaptionLength,
                nameof(caption),
                "Caption");

        ValidateDisplayOrder(displayOrder);

        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        AttachmentType = attachmentType;
        Caption = normalizedCaption;
        DisplayOrder = displayOrder;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void ChangeDisplayOrder(
        int displayOrder,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotDeleted();

        ValidateDisplayOrder(displayOrder);

        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        DisplayOrder = displayOrder;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void MarkAsPrimary(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotDeleted();

        if (IsPrimary)
        {
            throw new InvalidOperationException(
                "The attachment is already primary.");
        }

        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        IsPrimary = true;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void RemovePrimaryStatus(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotDeleted();

        if (!IsPrimary)
        {
            throw new InvalidOperationException(
                "The attachment is not primary.");
        }

        ValidateModification(
            modifiedAtUtc,
            modifiedByUserId);

        IsPrimary = false;

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

        IsPrimary = false;
        DeletionReason = normalizedDeletionReason;
        DeletedAtUtc = deletedAtUtc;
        DeletedByUserId = deletedByUserId;

        MarkModified(
            deletedAtUtc,
            deletedByUserId);
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
                "Attachment activity cannot occur before the attachment was created.",
                nameof(modifiedAtUtc));
        }
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "A deleted work order attachment cannot be changed.");
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

    private static void ValidateAttachmentType(
        WorkOrderAttachmentType attachmentType)
    {
        if (!Enum.IsDefined(attachmentType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(attachmentType),
                attachmentType,
                "Work order attachment type is invalid.");
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
