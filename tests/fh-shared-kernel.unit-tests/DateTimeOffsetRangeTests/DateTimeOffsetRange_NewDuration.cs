using FluentAssertions;
using LocalAuthorityInformationServices.SharedKernel.UnitTests.DateTimeRangeTests;

namespace LocalAuthorityInformationServices.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

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

