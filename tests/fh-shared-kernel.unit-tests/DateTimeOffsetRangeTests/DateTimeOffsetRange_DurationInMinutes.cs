using FamilyHubs.SharedKernel.UnitTests.DateTimeRangeTests;
using FluentAssertions;

namespace FamilyHubs.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

public class DateTimeOffsetRange_DurationInMinutes
{
    [Fact]
    public void Returns60GivenOneHourDifference()
    {
        var dtor = new DateTimeRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));

        dtor.DurationInMinutes().Should().Be(60);
    }
}
