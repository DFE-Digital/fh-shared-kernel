using AutoFixture;
using FamilyHubs.SharedKernel.GovLogin.Authentication;
using FamilyHubs.SharedKernel.UnitTests.GovLogin.TestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.UnitTests.GovLogin.Authentication
{
    public class AccountActiveAuthorizationHandlerTests
    {
        private string _role;
        private AccountActiveRequirement _requirement;
        private SharedKernel.GovLogin.Authentication.AccountActiveAuthorizationHandler _authorizationHandler;
        private IConfiguration _configuration;

        public AccountActiveAuthorizationHandlerTests()
        {
            var fixture = new Fixture();
            _role = fixture.Create<string>();
            _requirement = new AccountActiveRequirement();
            _configuration = FakeConfiguration.GetConfiguration();
            _authorizationHandler = new SharedKernel.GovLogin.Authentication.AccountActiveAuthorizationHandler(_configuration);
        }

        [Fact]
        public async Task HandleAsync_IfClaimDoesNotExist_ThenSucceeds()
        {
            //Arrange
            var httpContextBase = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            httpContextBase.Setup(c => c.Response).Returns(response.Object);
            var claim = new Claim("AccountSuspended", "true");
            var claimsPrinciple = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { claim }) });
            var context = new AuthorizationHandlerContext(new[] { _requirement }, claimsPrinciple, httpContextBase.Object);

            //Act
            await _authorizationHandler.HandleAsync(context);

            //Assert
            Assert.True(context.HasSucceeded);
            response.Verify(x => x.Redirect("/Errors/AccountSuspended"), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_IfClaimExists_And_NotSuspended_ThenSucceeds()
        {
            //Arrange
            var httpContextBase = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            httpContextBase.Setup(c => c.Response).Returns(response.Object);
            var claim = new Claim(ClaimTypes.AuthorizationDecision, "active");
            var claimsPrinciple = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { claim }) });
            var context = new AuthorizationHandlerContext(new[] { _requirement }, claimsPrinciple, httpContextBase.Object);

            //Act
            await _authorizationHandler.HandleAsync(context);

            //Assert
            Assert.True(context.HasSucceeded);
            response.Verify(x => x.Redirect("/Errors/AccountSuspended"), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_IfClaimExists_And_IsSuspended_ThenRedirects()
        {
            //Arrange

            var httpContextBase = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            httpContextBase.Setup(c => c.Response).Returns(response.Object);
            var claim = new Claim(ClaimTypes.AuthorizationDecision, "sUsPended");
            var claimsPrinciple = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { claim }) });
            var context = new AuthorizationHandlerContext(new[] { _requirement }, claimsPrinciple, httpContextBase.Object);

            //Act
            await _authorizationHandler.HandleAsync(context);

            //Assert
            Assert.True(context.HasSucceeded);
            response.Verify(x => x.Redirect("https://familyhubs-test.com/service/account-unavailable"));
        }
    }
}
