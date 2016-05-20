using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.AuthenticationTests.UserServiceTests
{
    public class WhenGettingProfileData : UserServiceTestBase
    {
        private ProfileDataRequestContext _requestContext;
        private ClaimsPrincipal _principal;

        public override void Arrange()
        {
            base.Arrange();

            _principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(Constants.ClaimTypes.Id,"xyz")
            }));

            _requestContext = new ProfileDataRequestContext
            {
                Subject = _principal
            };

            _mediator.Setup(m => m.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == "xyz")))
                .Returns(Task.FromResult(new User
                {
                    Id = "xyz",
                    FirstName = "Thomas",
                    LastName = "Tester",
                    Email = "thomas.tester@unittest.local",
                    Password = "some_hashed_password"
                }));
        }

        [Test]
        public async Task ThenItShouldReturnNoClaimsIfNoUserFound()
        {
            // Arrange
            _requestContext.Subject = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(Constants.ClaimTypes.Id,"zyx")
            }));

            // Act
            await _userService.GetProfileDataAsync(_requestContext);

            // Assert
            Assert.IsEmpty(_requestContext.IssuedClaims);
        }

        [Test]
        public async Task ThenItShouldReturnNoClaimsIfSubjectIsNull()
        {
            // Arrange
            _requestContext.Subject = null;

            // Act
            await _userService.GetProfileDataAsync(_requestContext);

            // Assert
            Assert.IsEmpty(_requestContext.IssuedClaims);
        }

        [Test]
        public async Task ThenItShouldReturnNoClaimsIfSubjectHasNoSubClaim()
        {
            // Arrange
            _requestContext.Subject = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("notsub","xyz")
            }));

            // Act
            await _userService.GetProfileDataAsync(_requestContext);

            // Assert
            Assert.IsEmpty(_requestContext.IssuedClaims);
        }

        [Test]
        public async Task ThenItShouldSendQueryWithUserSubClaimAsId()
        {
            // Act
            await _userService.GetProfileDataAsync(_requestContext);

            // Assert
            _mediator.Verify(m => m.SendAsync(It.Is<GetUserByIdQuery>((q) => q.UserId.Equals("xyz"))), Times.Once());
        }

        [Test]
        public async Task ThenItShouldReturnTheClaimsForTheUser()
        {
            // Act
            await _userService.GetProfileDataAsync(_requestContext);

            // Assert
            AssertClaimExistsAndHasCorrectValue(_requestContext.IssuedClaims, Constants.ClaimTypes.Id, "xyz");
            AssertClaimExistsAndHasCorrectValue(_requestContext.IssuedClaims, Constants.ClaimTypes.Email, "thomas.tester@unittest.local");
            AssertClaimExistsAndHasCorrectValue(_requestContext.IssuedClaims, Constants.ClaimTypes.GivenName, "Thomas");
            AssertClaimExistsAndHasCorrectValue(_requestContext.IssuedClaims, Constants.ClaimTypes.FamilyName, "Tester");
            AssertClaimExistsAndHasCorrectValue(_requestContext.IssuedClaims, Constants.ClaimTypes.Name, "Thomas Tester");
        }


        private void AssertClaimExistsAndHasCorrectValue(IEnumerable<Claim> claims, string expectedType, string expectedValue)
        {
            var claim = claims.FirstOrDefault(c => c.Type.Equals(expectedType));
            Assert.IsNotNull(claim, $"Expected claim of type {expectedType}, but none found");
            Assert.AreEqual(expectedValue, claim.Value, $"Expected claim of type {expectedType} to have value of {expectedValue}. Actual value {claim.Value}");
        }

    }
}
