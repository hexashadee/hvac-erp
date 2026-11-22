using NullSupportERP.Domain.Common.Events;

namespace NullSupportERP.Domain.Common.Base;

public abstract class AggregateRoot : AuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    protected AggregateRoot(
        Guid id,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(id, createdAtUtc, createdByUserId)
    {
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
