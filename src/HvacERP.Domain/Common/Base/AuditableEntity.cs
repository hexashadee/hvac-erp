namespace HvacERP.Domain.Common.Base;

public abstract class AuditableEntity : AggregateRoot
{
    public DateTime CreatedAtUtc { get; protected set; }

    public Guid CreatedByUserId { get; protected set; }

    public DateTime? LastModifiedAtUtc { get; protected set; }

    public Guid? LastModifiedByUserId { get; protected set; }

    protected AuditableEntity(Guid id)
        : base(id)
    {
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void SetCreatedBy(Guid userId)
    {
        CreatedByUserId = userId;
    }

    public void SetLastModified(Guid userId)
    {
        LastModifiedByUserId = userId;
        LastModifiedAtUtc = DateTime.UtcNow;
    }
}
