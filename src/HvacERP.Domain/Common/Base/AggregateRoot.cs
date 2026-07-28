namespace HvacERP.Domain.Common.Base;

public abstract class AggregateRoot : AuditableEntity
{
    protected AggregateRoot(
        Guid id,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(id, createdAtUtc, createdByUserId)
    {
    }
}
