using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.IsUserActive;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.AuthenticationTests.UserServiceTests
{
    public class WhenCheckingUserIsActive : UserServiceTestBase
    {
        private IsActiveContext _isActiveContext;
        private ClaimsPrincipal _principal;

        public override void Arrange()
        {
            base.Arrange();

            _principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(Constants.ClaimTypes.Id,"xyz")
            }));

            _isActiveContext = new IsActiveContext(_principal, new Client());

        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenItShouldSetContextIsActiveToResultOfQuery(bool isActive)
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<IsUserActiveQuery>())).Returns(Task.FromResult(isActive));

            // Act
            await _userService.IsActiveAsync(_isActiveContext);

            // Assert
            Assert.AreEqual(isActive, _isActiveContext.IsActive);
        }

        [Test]
        public async Task ThenItShouldSendQueryWithUserSubClaimAsId()
        {
            // Act
            await _userService.IsActiveAsync(_isActiveContext);

            // Assert
            _mediator.Verify(m => m.SendAsync(It.Is<IsUserActiveQuery>((q) => q.UserId.Equals("xyz"))), Times.Once());
        }

        [Test]
        public async Task ThenIsShouldReturnFalseIfSubjectIsNull()
        {
            // Arrange
            _isActiveContext.Subject = null;

            // Act
            await _userService.IsActiveAsync(_isActiveContext);

            // Assert
            Assert.IsFalse(_isActiveContext.IsActive);
        }

        [Test]
        public async Task ThenIsShouldReturnFalseIfSubjectHasNoSubClaim()
        {
            // Arrange
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("notsub","xyz")
            }));
            var isActiveContext = new IsActiveContext(principal, new Client());

            // Act
            await _userService.IsActiveAsync(isActiveContext);

            // Assert
            Assert.IsFalse(isActiveContext.IsActive);
        }

    }
}
