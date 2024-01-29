using FamilyHubs.SharedKernel.Services.Postcode.Model;

namespace FamilyHubs.SharedKernel.Services.Postcode.Interfaces;

public interface IPostcodeLookup
{
    /// <returns>
    /// PostcodeError 
    /// IPostcodeInfo or null if there was an error.
    /// </returns>
    Task<(PostcodeError, IPostcodeInfo?)> Get(string? postcode, CancellationToken cancellationToken = default);
}