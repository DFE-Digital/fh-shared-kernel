using FluentAssertions;

namespace FamilyHubs.SharedKernel.UnitTests.BaseDomainEventTests;

public class BaseDomainEvent_Constructor
{
    public class TestEvent : DomainEventBase
    { }

    [Fact]
    public void SetsTimeToCurrentTime()
    {
        var newEvent = new TestEvent();

        newEvent.DateOccurred.Should().BeCloseTo(DateTime.UtcNow, new TimeSpan(100000));
    }
}
