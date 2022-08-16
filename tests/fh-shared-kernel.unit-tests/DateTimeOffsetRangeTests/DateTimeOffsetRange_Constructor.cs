using FamilyHubs.SharedKernel.UnitTests.DateTimeRangeTests;

namespace FamilyHubs.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

public class DateTimeOffsetRange_Constructor
{
    [Fact]
    public void ThrowsGivenStartAfterEnd()
    {
        Assert.Throws<ArgumentException>(() => new DateTimeOffsetRange(DateTimes.TestDateTime, DateTimes.TestDateTime.AddDays(-1)));
    }

    [Fact]
    public void ThrowsGivenStartAndNegativeTimeSpan()
    {
        TimeSpan span = new TimeSpan(-1, 0, 0);
        Assert.Throws<ArgumentException>(() => new DateTimeOffsetRange(DateTimes.TestDateTime, span));
    }
}
