namespace HvacERP.Domain.Common.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredAtUtc { get; }
}
