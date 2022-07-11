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
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.UserControllerTests
{
    [TestFixture]
    public class WhenIGetAUser
    {
        [Test, MoqAutoData]
        public async Task ThenTheUsersDetailsAreReturned(
           string userId,
           User user,
           [Frozen] Mock<ILogger> loggerMock,
           [Frozen] Mock<IMediator> mediator,
           [Frozen] Mock<IMapper> mapperMock,
           UserOrchestrator userOrchestrator)
        {
            // Arrange
            var mapperReal = new MapperConfiguration(c => c.AddProfile<DefaultProfile>()).CreateMapper();

            user.Id = userId;
            var controller = new UserController(userOrchestrator);
            mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId))).ReturnsAsync(user);
            mapperMock.Setup(mock => mock.Map<UserViewModel>(It.Is<User>(u => user.Id == userId))).Returns(mapperReal.Map<UserViewModel>(user));

            // Act
            var response = await controller.Show(userId);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<UserViewModel>>(response);
            var model = response as OkNegotiatedContentResult<UserViewModel>;

            model.Content.ShouldBeEquivalentTo(user, options => options.ExcludingMissingMembers());

            loggerMock.Verify(x => x.Info($"Getting user account {userId}."), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task AndTheUserDoesntExistThenTheUserIsNotFound(
          string userId,
          [Frozen] Mock<IMediator> mediator,
          [Frozen] Mock<IMapper> mapper,
          UserOrchestrator userOrchestrator)
        {
            // Arrange
            var controller = new UserController(userOrchestrator);
            mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId))).ReturnsAsync((User)null);
            mapper.Setup(x => x.Map<UserViewModel>(It.Is<User>(u => u == null))).Returns((UserViewModel)null);

            // Act
            var response = await controller.Show(userId);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }
}
