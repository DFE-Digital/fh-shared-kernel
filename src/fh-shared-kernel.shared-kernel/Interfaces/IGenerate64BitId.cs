using IdGen;

namespace FamilyHubs.SharedKernel.Interfaces
{
    public interface IIdGen64
    {
        IdGenerator? Generator { get; }

        long NewId();
    }
}