using HvacERP.Domain.Common.Base;

namespace HvacERP.Application.Common.Persistence;

public interface IRepository<TAggregate>
    where TAggregate : AggregateRoot
{
    Task<TAggregate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);

    void Remove(TAggregate aggregate);
}
