using FamilyHubs.SharedKernel.UnitTests.DateTimeRangeTests;
using FluentAssertions;

namespace FamilyHubs.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

public class DateTimeOffsetRange_Overlaps
{
    [Fact]
    public void ReturnsTrueGivenSameDateTimeRange()
    {
        var dtor = new DateTimeOffsetRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));

        var result = dtor.Overlaps(dtor);

        result.Should().BeTrue();
    }

    [Fact]
    public void ReturnsTrueGivenEarlierRangeExceedingStart()
    {
        var dtr = new DateTimeOffsetRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));
        var earlier = new DateTimeOffsetRange(DateTimes.TestDateTime.AddMinutes(-5), TimeSpan.FromMinutes(10));

        var result = dtr.Overlaps(earlier);

        result.Should().BeTrue();
    }

    [Fact]
    public void ReturnsTrueGivenRangeWithinRange()
    {
        var dtr = new DateTimeOffsetRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));
        var within = new DateTimeOffsetRange(DateTimes.TestDateTime.AddMinutes(5), TimeSpan.FromMinutes(10));

        var result = dtr.Overlaps(within);

        result.Should().BeTrue();
    }

    [Fact]
    public void ReturnsTrueGivenRangeStartingWithinRangeEndingLater()
    {
        var dtr = new DateTimeOffsetRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));
        var endslater = new DateTimeOffsetRange(dtr.End.AddMinutes(-5), TimeSpan.FromMinutes(10));

        var result = dtr.Overlaps(endslater);

        result.Should().BeTrue();
    }

    [Fact]
    public void ReturnsFalseGivenRangeEndingBeforeStart()
    {
        var dtr = new DateTimeOffsetRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));
        var endsearlier = new DateTimeOffsetRange(dtr.Start.AddHours(-1), TimeSpan.FromMinutes(10));

        var result = dtr.Overlaps(endsearlier);

        result.Should().BeFalse();
    }

    [Fact]
    public void ReturnsFalseGivenRangeStartingAfterEnd()
    {
        var dtr = new DateTimeOffsetRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));
        var startslater = new DateTimeOffsetRange(dtr.End.AddMinutes(1), TimeSpan.FromMinutes(10));

        var result = dtr.Overlaps(startslater);

        result.Should().BeFalse();
    }

}
