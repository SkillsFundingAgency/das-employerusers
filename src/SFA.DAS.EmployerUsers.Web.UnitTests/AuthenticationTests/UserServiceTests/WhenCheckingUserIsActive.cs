using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.ApplicationLayer.Queries.IsUserActive;

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
                new Claim("sub","xyz")
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
    }
}
