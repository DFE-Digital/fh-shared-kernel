using FluentAssertions;
using LocalAuthorityInformationServices.SharedKernel.UnitTests.DateTimeRangeTests;
using Xunit;

namespace LocalAuthorityInformationServices.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

public class DateTimeOffsetRange_CreateOneDayRange
{
    [Fact]
    public void CreatesRangeWithStartDateLastingOneDay()
    {
        var dtor = DateTimeOffsetRange.CreateOneDayRange(DateTimes.TestDateTime);

        dtor.Start.Should().Be(DateTimes.TestDateTime);
        dtor.End.Should().Be(dtor.Start.AddDays(1));
    }
}
