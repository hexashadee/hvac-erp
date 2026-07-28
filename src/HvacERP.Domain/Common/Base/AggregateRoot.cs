namespace HvacERP.Domain.Common.Base;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id)
        : base(id)
    {
    }
}
