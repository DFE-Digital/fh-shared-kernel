using FamilyHubs.SharedKernel.UnitTests.DateTimeRangeTests;
using FluentAssertions;

namespace FamilyHubs.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

public class DateTimeOffsetRange_NewStart
{
    [Fact]
    public void ReturnsNewObjectWithGivenEndDate()
    {
        DateTime newStartTime = DateTimes.TestDateTime.AddHours(-1);
        var dtor = new DateTimeOffsetRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));

        var newDtr = dtor.NewStart(newStartTime);

        dtor.Should().NotBeSameAs(newDtr);
        newDtr.Start.Should().Be(newStartTime);
    }
}

