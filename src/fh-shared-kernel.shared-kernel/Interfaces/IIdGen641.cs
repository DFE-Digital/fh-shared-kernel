using IdGen;

namespace FamilyHubs.SharedKernel.Interfaces
{
    public interface IIdGen641
    {
        IdGenerator CreateIdGenerator();
        long NewId();
    }
}