using FluentAssertions;

namespace FamilyHubs.SharedKernel.UnitTests.IdGenerator;

public class IdGen64_Constructor
{
    [Fact]
    public void ConstructsIdGenerator()
    {
        var idGenerator = new IdGen64();

        idGenerator.Should().NotBeNull();
    }
}
