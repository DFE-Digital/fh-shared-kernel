using Ardalis.Specification;

namespace FamilyHubs.SharedKernel.Interfaces;


public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
}
