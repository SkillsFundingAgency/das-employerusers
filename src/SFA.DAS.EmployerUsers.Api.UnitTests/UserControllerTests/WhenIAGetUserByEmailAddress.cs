using System.Threading.Tasks;
using System.Web.Http.Results;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Controllers;
using SFA.DAS.EmployerUsers.Api.DependencyResolution;
using SFA.DAS.EmployerUsers.Api.Orchestrators;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByEmailAddress;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.UserControllerTests
{
    [TestFixture]
    public class WhenIGetAUserByEmailAddress
    {
        [Test, MoqAutoData]
        public async Task ThenTheUsersDetailsAreReturned(
           string userEmail,
           User user,
           [Frozen] Mock<ILogger> loggerMock,
           [Frozen] Mock<IMediator> mediator,
           [Frozen] Mock<IMapper> mapperMock,
           UserOrchestrator userOrchestrator)
        {
            // Arrange
            var mapperReal = new MapperConfiguration(c => c.AddProfile<DefaultProfile>()).CreateMapper();

            user.Email = userEmail;
            var controller = new UserController(userOrchestrator);
            mediator.Setup(x => x.SendAsync(It.Is<GetUserByEmailAddressQuery>(q => q.EmailAddress == userEmail))).ReturnsAsync((User)null);
            mapperMock.Setup(mock => mock.Map<UserViewModel>(It.Is<User>(u => user.Email == userEmail))).Returns(mapperReal.Map<UserViewModel>(user));

            // Act
            var response = await controller.Email(userEmail);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<UserViewModel>>(response);
            var model = response as OkNegotiatedContentResult<UserViewModel>;

            model.Content.ShouldBeEquivalentTo(user, options => options.ExcludingMissingMembers());

            loggerMock.Verify(x => x.Info($"Getting user account for email address {userEmail}."), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task AndTheUserDoesntExistThenTheUserIsNotFound(
          string userEmail,
          [Frozen] Mock<IMediator> mediator,
          [Frozen] Mock<IMapper> mapper,
          UserOrchestrator userOrchestrator)
        {
            // Arrange
            var controller = new UserController(userOrchestrator);
            mediator.Setup(x => x.SendAsync(It.Is<GetUserByEmailAddressQuery>(u => u.EmailAddress == userEmail))).ReturnsAsync((User)null);
            mapper.Setup(x => x.Map<UserViewModel>(It.Is<User>(u => u == null))).Returns((UserViewModel)null);

            // Act
            var response = await controller.Email(userEmail);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }
}
