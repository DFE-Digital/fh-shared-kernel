using System.Threading.Tasks;

namespace LocalAuthorityInformationServices.SharedKernel.Interfaces;

public interface IHandle<T> where T : DomainEventBase
{
    Task HandleAsync(T args);
}
