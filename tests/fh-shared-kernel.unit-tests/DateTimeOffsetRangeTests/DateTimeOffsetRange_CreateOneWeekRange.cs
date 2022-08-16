using FamilyHubs.SharedKernel;
using FamilyHubs.SharedKernel.UnitTests.DateTimeRangeTests;
using FluentAssertions;

namespace FamilyHubs.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

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
