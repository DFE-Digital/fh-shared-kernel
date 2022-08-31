namespace FamilyHubs.SharedKernel.UnitTests.DateTimeRangeTests;

public class DateTimeRange_Constructor
{
    [Fact]
    public void ThrowsGivenStartAfterEnd()
    {
        Assert.Throws<ArgumentException>(() => new DateTimeRange(DateTimes.TestDateTime, DateTimes.TestDateTime.AddDays(-1)));
    }

    [Fact]
    public void ThrowsGivenStartAndNegativeTimeSpan()
    {
        TimeSpan span = new TimeSpan(-1, 0, 0);
        Assert.Throws<ArgumentException>(() => new DateTimeRange(DateTimes.TestDateTime, span));
    }
}
