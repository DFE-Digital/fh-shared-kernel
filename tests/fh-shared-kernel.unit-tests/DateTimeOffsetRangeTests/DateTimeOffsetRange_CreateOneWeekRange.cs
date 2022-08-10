using FluentAssertions;
using LocalAuthorityInformationServices.SharedKernel.UnitTests.DateTimeRangeTests;
using Xunit;

namespace LocalAuthorityInformationServices.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

public class DateTimeOffsetRange_CreateOneWeekRange
{
    [Fact]
    public void CreatesRangeWithStartDateLastingSevenDay()
    {
        var dtor = DateTimeOffsetRange.CreateOneWeekRange(DateTimes.TestDateTime);

        dtor.Start.Should().Be(DateTimes.TestDateTime);
        dtor.End.Should().Be(dtor.Start.AddDays(7));
    }
}
