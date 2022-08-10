using Ardalis.Specification;

namespace LocalAuthorityInformationServices.SharedKernel.Interfaces;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
}
