using FamilyHubs.SharedKernel.GovLogin.Configuration;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Identity.Authentication.Gov;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Security.Principal;

namespace FamilyHubs.SharedKernel.UnitTests.Identity.Authentication.Gov
{
    public class AccountMiddlewareTests
    {
        private GovUkOidcConfiguration _configuration;
        private RequestDelegate _nextMock;
        private ILogger<AccountMiddleware> _mockedLogger;

        public AccountMiddlewareTests()
        {
            _configuration = new GovUkOidcConfiguration { Oidc = new Oidc() };
            _configuration.BearerTokenSigningKey = Guid.NewGuid().ToString();
            _configuration.Oidc.PrivateKey = Guid.NewGuid().ToString();
            _nextMock = Mock.Of<RequestDelegate>();
            _mockedLogger = Mock.Of<ILogger<AccountMiddleware>>();
        }

        [Fact]
        public async Task InvokeAsync_UserNull_NoTokenSet()
        {
            //  Arrange
            var mockContext = CreateMockHttpContext();
            var context = mockContext.Object;
            var accountMiddleware = new AccountMiddleware(_nextMock, _configuration, _mockedLogger);

            //  Act
            await accountMiddleware.InvokeAsync(context);

            //  Assert
            var bearerToken = context.GetBearerToken();
            Assert.True(string.IsNullOrEmpty(bearerToken));
        }

        [Fact]
        public async Task InvokeAsync_UserNotAuthenticated_NoTokenSet()
        {
            //  Arrange
            var mockContext = CreateMockHttpContext();
            mockContext.Setup(m => m.User).Returns(CreateUser(false));
            var context = mockContext.Object;
            var accountMiddleware = new AccountMiddleware(_nextMock, _configuration, _mockedLogger);

            //  Act
            await accountMiddleware.InvokeAsync(context);

            //  Assert
            var bearerToken = context.GetBearerToken();
            Assert.True(string.IsNullOrEmpty(bearerToken));
        }

        [Fact]
        public async Task InvokeAsync_SetsBearerToken()
        {
            //  Arrange
            var mockContext = CreateMockHttpContext();
            mockContext.Setup(m => m.User).Returns(CreateUser(true));
            var context = mockContext.Object;
            var accountMiddleware = new AccountMiddleware(_nextMock, _configuration, _mockedLogger);

            //  Act
            await accountMiddleware.InvokeAsync(context);

            //  Assert
            var bearerToken = context.GetBearerToken();
            Assert.False(string.IsNullOrEmpty(bearerToken));
        }

        private Mock<HttpContext> CreateMockHttpContext()
        {
            var mockHttpContext = new Mock<HttpContext>();

            var items = new Dictionary<object, object?>();
            mockHttpContext.Setup(m => m.Items).Returns(items);

            var request = new Mock<HttpRequest>();
            request.SetupGet(m => m.Path).Returns("/somepath/action");
            mockHttpContext.Setup(m => m.Request).Returns(request.Object);
            return mockHttpContext;
        }

        private ClaimsPrincipal CreateUser(bool isAuthenticated)
        {
            var mockUser = new Mock<ClaimsPrincipal>();
            var mockIdentity = new Mock<IIdentity>();

            mockIdentity.Setup(m => m.IsAuthenticated).Returns(isAuthenticated);
            mockUser.Setup(m => m.Identity).Returns(mockIdentity.Object);

            return mockUser.Object;
        }
    }
}
