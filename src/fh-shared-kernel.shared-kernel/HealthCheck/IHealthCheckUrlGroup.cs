using Microsoft.Extensions.Configuration;

namespace FamilyHubs.SharedKernel.HealthCheck;

public interface IHealthCheckUrlGroup
{
    static abstract Uri HealthUrl(IConfiguration configuration);
}