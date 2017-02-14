using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.UserControllerTests
{
    [TestFixture]
    public class WhenIGetAUser : UserControllerTestsBase
    {
        [Test]
        public async Task ThenTheUsersDetailsAreReturned()
        {
            var userId = "124";

            var user = new User
            {
                Email = "a@b.com",
                LastName = "Bloggs",
                FirstName = "Joe",
                Id = userId,
                FailedLoginAttempts = 2,
                IsActive = true,
                IsLocked = true,
                Password = "********",
                Salt = "ABC123",
                PasswordProfileId = "ZZZ999"
            };

            Mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId))).ReturnsAsync(user);

            var response = await Controller.Show(userId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<UserViewModel>>(response);
            var model = response as OkNegotiatedContentResult<UserViewModel>;

            model.Content.ShouldBeEquivalentTo(user, options => options.ExcludingMissingMembers());

            Logger.Verify(x => x.Info($"Getting user account {userId}."), Times.Once);
        }

        [Test]
        public async Task AndTheUserDoesntExistThenTheUserIsNotFound()
        {
            var userId = "999";

            Mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId))).ReturnsAsync((User)null);

            var response = await Controller.Show(userId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }
}
