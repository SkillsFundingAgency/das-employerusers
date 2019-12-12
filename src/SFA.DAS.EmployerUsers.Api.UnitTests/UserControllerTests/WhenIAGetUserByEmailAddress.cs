using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByEmailAddress;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.UserControllerTests
{
    [TestFixture]
    public class WhenIGetAUserByEmailAddress : UserControllerTestsBase
    {
        [Test]
        public async Task ThenTheUsersDetailsAreReturned()
        {
            var emailAddress = "a@b.com";

            var user = new User
            {
                Email = emailAddress,
                LastName = "Bloggs",
                FirstName = "Joe",
                Id = "1234",
                FailedLoginAttempts = 2,
                IsActive = true,
                IsLocked = true,
                Password = "********",
                Salt = "ABC123",
                PasswordProfileId = "ZZZ999"
            };
            
            Mediator.Setup(x => x.SendAsync(It.Is<GetUserByEmailAddressQuery>(q => q.EmailAddress == emailAddress))).ReturnsAsync(user);

            var response = await Controller.Email(emailAddress);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<UserViewModel>>(response);
            var model = response as OkNegotiatedContentResult<UserViewModel>;

            model.Content.ShouldBeEquivalentTo(user, options => options.ExcludingMissingMembers());

            Logger.Verify(x => x.Info($"Getting user account for email address {emailAddress}."), Times.Once);
        }

        [Test]
        public async Task AndTheUserDoesntExistThenTheUserIsNotFound()
        {
            var emailAddress = "x@y.com";

            Mediator.Setup(x => x.SendAsync(It.Is<GetUserByEmailAddressQuery>(q => q.EmailAddress == emailAddress))).ReturnsAsync((User) null);

            var response = await Controller.Email(emailAddress);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }
}
