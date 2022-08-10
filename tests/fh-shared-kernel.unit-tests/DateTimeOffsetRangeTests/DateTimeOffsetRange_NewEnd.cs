using FluentAssertions;
using LocalAuthorityInformationServices.SharedKernel.UnitTests.DateTimeRangeTests;

namespace LocalAuthorityInformationServices.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

public class DateTimeOffsetRange_NewEnd
{
    [Fact]
    public void ReturnsNewObjectWithGivenEndDate()
    {
        DateTime newEndTime = DateTimes.TestDateTime.AddHours(2);
        var dtor = new DateTimeOffsetRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));

        var newDtr = dtor.NewEnd(newEndTime);

        dtor.Should().NotBeSameAs(newDtr);
        newDtr.End.Should().Be(newEndTime);
    }
}

