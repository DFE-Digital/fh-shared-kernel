using FamilyHubs.SharedKernel.UnitTests.DateTimeRangeTests;
using FluentAssertions;

namespace FamilyHubs.SharedKernel.UnitTests.DateTimeOffsetRangeTests;

public class DateTimeOffsetRange_Equality
{
    [Fact]
    public void ReturnsTrueGivenSameStartEnd()
    {
        var dtor1 = new DateTimeOffsetRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));
        var dtor2 = new DateTimeOffsetRange(DateTimes.TestDateTime, TimeSpan.FromHours(1));

        dtor1.Should().NotBeSameAs(dtor2);
        dtor1.Should().BeEquivalentTo(dtor2);
        dtor2.Should().BeEquivalentTo(dtor1);
        (dtor1 == dtor2).Should().BeTrue();
    }
}

