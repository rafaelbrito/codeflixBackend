using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Domain.SeedWork
{
    public class AggregateRootTest
    {
        [Fact(DisplayName = nameof(RaiseEvent))]
        [Trait("Domain", "AggregateRoot")]
        public void RaiseEvent()
        {
            var domainEvent = new DomainEventFake();
            var aggregate = new AggregateRootFake();

            aggregate.RaiseEvent(domainEvent);

            aggregate.Events.Should().HaveCount(1);
        }

        [Fact(DisplayName = nameof(ClearEvent))]
        [Trait("Domain", "AggregateRoot")]
        public void ClearEvent()
        {
            var domainEvent = new DomainEventFake();
            var aggregate = new AggregateRootFake();
            aggregate.RaiseEvent(domainEvent);

            aggregate.ClearEvents();
            aggregate.Events.Should().BeEmpty();
        }
    }
}