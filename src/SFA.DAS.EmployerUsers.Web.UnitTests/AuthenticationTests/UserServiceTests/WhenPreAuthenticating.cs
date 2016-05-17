using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using IdentityServer3.Core.Models;
using NUnit.Framework;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.AuthenticationTests.UserServiceTests
{
    public class WhenPreAuthenticating : UserServiceTestBase
    {
        private PreAuthenticationContext _preAuthenticationContext;

        public override void Arrange()
        {
            base.Arrange();

            _requestQueryString.Add("signin", new[] {"test-signin-id"});

            _preAuthenticationContext = new PreAuthenticationContext
            {
                SignInMessage = new SignInMessage
                {
                    ReturnUrl = "http://some-relying-party.local",
                    ClientId = "UnitTests"
                }
            };
        }

        [Test]
        public async Task ThenItShouldSetContextAuthenticateResultToInstance()
        {
            // Act
            await _userService.PreAuthenticateAsync(_preAuthenticationContext);

            // Assert
            Assert.IsNotNull(_preAuthenticationContext.AuthenticateResult);
        }

        [Test]
        public async Task ThenItShouldSetTheReturnUrlToTheCorrectFormat()
        {
            // Act
            await _userService.PreAuthenticateAsync(_preAuthenticationContext);

            // Assert
            var expectedFormat = @"~\/employer\/login\?id=.{1,}&returnUrl=.{1,}&clientId=.{1,}";
            Assert.IsTrue(Regex.IsMatch(_preAuthenticationContext.AuthenticateResult.PartialSignInRedirectPath, expectedFormat));
        }

        [Test]
        public async Task ThenItShouldIncludeTheSigninIdInTheUrl()
        {
            // Act
            await _userService.PreAuthenticateAsync(_preAuthenticationContext);

            // Assert
            Assert.True(_preAuthenticationContext.AuthenticateResult.PartialSignInRedirectPath.Contains("id=test-signin-id"));
        }

        [Test]
        public async Task ThenItShouldIncludeTheReturnUrlInTheUrl()
        {
            // Act
            await _userService.PreAuthenticateAsync(_preAuthenticationContext);

            // Assert
            var expectedReturnUrlValue = HttpUtility.UrlEncode("http://some-relying-party.local");
            Assert.True(_preAuthenticationContext.AuthenticateResult.PartialSignInRedirectPath.Contains("returnUrl=" + expectedReturnUrlValue));
        }

        [Test]
        public async Task ThenItShouldIncludeTheClientIdInTheUrl()
        {
            // Act
            await _userService.PreAuthenticateAsync(_preAuthenticationContext);

            // Assert
            Assert.True(_preAuthenticationContext.AuthenticateResult.PartialSignInRedirectPath.Contains("clientId=UnitTests"));
        }
    }
}
