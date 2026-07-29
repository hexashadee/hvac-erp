namespace HvacERP.Domain.WorkOrders;

public sealed class WorkOrderNote
{
    public Guid Id { get; private set; }

    public WorkOrderNoteType Type { get; private set; }

    public string Content { get; private set; }

    public bool IsPinned { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public Guid? LastModifiedByUserId { get; private set; }

    public DateTimeOffset? LastModifiedAtUtc { get; private set; }

    public bool IsDeleted { get; private set; }

    public Guid? DeletedByUserId { get; private set; }

    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public string? DeletionReason { get; private set; }

    public bool IsCustomerVisible =>
        Type == WorkOrderNoteType.CustomerVisible;

    internal WorkOrderNote(
        Guid id,
        WorkOrderNoteType type,
        string content,
        bool isPinned,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        Type = type;
        Content = content;
        IsPinned = isPinned;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = createdAtUtc;
    }

    internal void Update(
        WorkOrderNoteType type,
        string content,
        bool isPinned,
        Guid modifiedByUserId,
        DateTimeOffset modifiedAtUtc)
    {
        Type = type;
        Content = content;
        IsPinned = isPinned;
        LastModifiedByUserId = modifiedByUserId;
        LastModifiedAtUtc = modifiedAtUtc;
    }

    internal void Delete(
        string deletionReason,
        Guid deletedByUserId,
        DateTimeOffset deletedAtUtc)
    {
        IsDeleted = true;
        DeletedByUserId = deletedByUserId;
        DeletedAtUtc = deletedAtUtc;
        DeletionReason = deletionReason;
        IsPinned = false;
    }
}
