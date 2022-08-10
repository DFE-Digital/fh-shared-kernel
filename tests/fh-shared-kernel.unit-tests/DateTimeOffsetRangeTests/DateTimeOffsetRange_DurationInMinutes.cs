using FluentAssertions;
using LocalAuthorityInformationServices.SharedKernel.UnitTests.DateTimeRangeTests;

namespace LocalAuthorityInformationServices.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

public class DateTimeOffsetRange_DurationInMinutes
{
    [Fact]
    public void Returns60GivenOneHourDifference()
    {
        var dtor = new DateTimeRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));

        dtor.DurationInMinutes().Should().Be(60);
    }
}
