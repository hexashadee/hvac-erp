using NullSupportERP.Domain.Common.Base;
using NullSupportERP.Domain.Common.Events;

namespace NullSupportERP.Domain.Tests.Common.Events;

public sealed class AggregateRootTests
{
    [Fact]
    public void RaiseDomainEvent_AddsEventToCollection()
    {
        TestAggregate aggregate = CreateAggregate();
        TestDomainEvent domainEvent = new(DateTimeOffset.UtcNow);

        aggregate.Record(domainEvent);

        IDomainEvent recordedEvent = Assert.Single(aggregate.DomainEvents);
        Assert.Same(domainEvent, recordedEvent);
    }

    [Fact]
    public void RaiseDomainEvent_WithMultipleEvents_PreservesOrder()
    {
        TestAggregate aggregate = CreateAggregate();

        TestDomainEvent firstEvent = new(DateTimeOffset.UtcNow);
        TestDomainEvent secondEvent = new(DateTimeOffset.UtcNow.AddSeconds(1));

        aggregate.Record(firstEvent);
        aggregate.Record(secondEvent);

        Assert.Collection(
            aggregate.DomainEvents,
            item => Assert.Same(firstEvent, item),
            item => Assert.Same(secondEvent, item));
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        TestAggregate aggregate = CreateAggregate();

        aggregate.Record(new TestDomainEvent(DateTimeOffset.UtcNow));
        aggregate.Record(new TestDomainEvent(DateTimeOffset.UtcNow.AddSeconds(1)));

        aggregate.ClearDomainEvents();

        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void RaiseDomainEvent_WithNullEvent_ThrowsArgumentNullException()
    {
        TestAggregate aggregate = CreateAggregate();

        Assert.Throws<ArgumentNullException>(() => aggregate.Record(null!));
    }

    private static TestAggregate CreateAggregate()
    {
        return new TestAggregate(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            Guid.NewGuid());
    }

    private sealed class TestAggregate : AggregateRoot
    {
        public TestAggregate(
            Guid id,
            DateTimeOffset createdAtUtc,
            Guid createdByUserId)
            : base(id, createdAtUtc, createdByUserId)
        {
        }

        public void Record(IDomainEvent domainEvent)
        {
            RaiseDomainEvent(domainEvent);
        }
    }

    private sealed record TestDomainEvent(
        DateTimeOffset OccurredAtUtc) : IDomainEvent;
}
