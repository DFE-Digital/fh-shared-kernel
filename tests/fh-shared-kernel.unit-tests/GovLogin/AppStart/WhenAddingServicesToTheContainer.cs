using FamilyHubs.SharedKernel.GovLogin.AppStart;
using FamilyHubs.SharedKernel.GovLogin.Authentication;
using FamilyHubs.SharedKernel.GovLogin.Services;
using FamilyHubs.SharedKernel.UnitTests.GovLogin.TestHelpers;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.UnitTests.GovLogin.AppStart
{
    public class WhenAddingServicesToTheContainer
    {
        [Theory]
        [InlineData(typeof(IOidcService))]
        [InlineData(typeof(IAzureIdentityService))]
        [InlineData(typeof(IJwtSecurityTokenService))]
        [InlineData(typeof(ICustomClaims))]
        [InlineData(typeof(IStubAuthenticationService))]
        public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
        {
            var serviceCollection = new ServiceCollection();
            SetupServiceCollection(serviceCollection);

            var provider = serviceCollection.BuildServiceProvider();

            var type = provider.GetService(toResolve);

            Assert.NotNull(type);
        }

        [Fact]
        public void Then_Resolves_Authorization_Handlers()
        {
            var serviceCollection = new ServiceCollection();
            SetupServiceCollection(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            var type = provider.GetServices(typeof(IAuthorizationHandler)).ToList();

            Assert.NotNull(type);
            Assert.IsType<AccountActiveAuthorizationHandler>(type.Single());
        }


        private static void SetupServiceCollection(IServiceCollection serviceCollection)
        {
            var configuration = FakeConfiguration.GetConfiguration();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddAndConfigureGovUkAuthentication(configuration, "authenticationCookieName");
        }



        public class TestCustomClaims : ICustomClaims
        {
            public Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
            {
                throw new NotImplementedException();
            }
        }
    }
}
