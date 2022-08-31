using FamilyHubs.SharedKernel.UnitTests.DateTimeRangeTests;
using FluentAssertions;

namespace FamilyHubs.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

public class DateTimeOffsetRange_NewDuration
{
    [Fact]
    public void ReturnsNewObjectWithGivenDuration()
    {
        var dtor = new DateTimeOffsetRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));

        var newDtr = dtor.NewDuration(TimeSpan.FromHours(2));

        dtor.Should().NotBeSameAs(newDtr);
        newDtr.DurationInMinutes().Should().Be(120);
    }
}

