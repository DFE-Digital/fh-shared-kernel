using FluentAssertions;

namespace FamilyHubs.SharedKernel.UnitTests.IdGenerator;

public class IdGen64_GeneratorId
{
    [Fact]
    public void GeneraterIdShouldBeOne()
    {
        var idGenerator = new IdGen64();
        var t = idGenerator.CreateIdGenerator();
        var id = idGenerator.NewId();
        var genid = t.FromId(id);

        genid.GeneratorId.Should().Be(1);
    }
}