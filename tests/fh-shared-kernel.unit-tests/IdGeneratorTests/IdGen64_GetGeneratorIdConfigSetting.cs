using FluentAssertions;

namespace FamilyHubs.SharedKernel.UnitTests.IdGenerator;

public class IdGen64_GetGeneratorIdConfigSetting
{
    [Fact]
    public void GeneratorIdSettingShouldBeSet()
    {
        var generatorIdSetting = IdGen64.GetGeneratorIdConfigSetting();

        generatorIdSetting.Should().NotBeNull();
        generatorIdSetting.Should().Be("1");
    }
}