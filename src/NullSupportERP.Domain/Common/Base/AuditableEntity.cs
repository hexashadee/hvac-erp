namespace NullSupportERP.Domain.Common.Base;

public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public DateTimeOffset? LastModifiedAtUtc { get; private set; }

    public Guid? LastModifiedByUserId { get; private set; }

    protected AuditableEntity(
        Guid id,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(id)
    {
        ValidateUserId(createdByUserId);

        CreatedAtUtc = createdAtUtc.ToUniversalTime();
        CreatedByUserId = createdByUserId;
    }

    public void MarkModified(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateUserId(modifiedByUserId);

        LastModifiedAtUtc = modifiedAtUtc.ToUniversalTime();
        LastModifiedByUserId = modifiedByUserId;
    }

    private static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }
    }
}
