using FluentAssertions;

namespace FamilyHubs.SharedKernel.UnitTests.IdGenerator;

public class IdGen64_GetConfig
{
    [Fact]
    public void GeneratorConfigShouldBeSet()
    {
        var idGen64Config = IdGen64.GetConfig();

        idGen64Config?.Structure?.TimestampBits.Should().Be(45);
        idGen64Config?.Structure?.GeneratorIdBits.Should().Be(3);
        idGen64Config?.Structure?.SequenceBits.Should().Be(15);
        idGen64Config?.Epoch.Should().Be(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        idGen64Config?.GeneratorId.Should().Be(1);
    }
}