using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Controllers;
using SFA.DAS.EmployerUsers.Api.Orchestrators;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Commands.ResumeUser;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.UserControllerTests
{
    [TestFixture]
    public class WhenIResumeAUser
    {
        [Test, MoqAutoData]
        public async Task WhenException_ThenInternalServerError(
            string userId,
            ChangedByUserInfo changedByUserInfo,
            [Frozen] Mock<IMediator> mediator,
            UserOrchestrator userOrchestrator)
        {
            // Arrange
            var controller = new UserController(userOrchestrator);
            mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId)))
                .Throws(new Exception("Fail"));

            // Act
            var response = await controller.Resume(userId, changedByUserInfo);

            // Assert
            response.Should().BeOfType<InternalServerErrorResult>();
        }

        [Test, MoqAutoData]
        public async Task WhenUserDoesNotExist_ThenANotFoundResultIsReturned(
            string userId,
            [Frozen] Mock<IMediator> mediator,
            ChangedByUserInfo changedByUserInfo,
            UserOrchestrator userOrchestrator)
        {
            // Arrange
            var controller = new UserController(userOrchestrator);

            mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId)))
                .ReturnsAsync((User)null);

            // Act
            var response = await controller.Resume(userId, changedByUserInfo);

            // Assert
            response.Should().BeOfType<NotFoundResult>();
        }

        [Test, MoqAutoData]
        public async Task ThenShouldFetchUser(
            User user,
           string userId,
           [Frozen] Mock<IMediator> mediator,
           ChangedByUserInfo changedByUserInfo,
           UserOrchestrator userOrchestrator)
        {
            // Arrange
            var controller = new UserController(userOrchestrator);

            mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId)))
                .ReturnsAsync(user)
                .Verifiable();

            // Act
            var response = await controller.Resume(userId, changedByUserInfo);

            // Assert
            mediator.Verify(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task WhenUserNotFound_ThenShouldNotResume(
          string userId,
          [Frozen] Mock<IMediator> mediator,
          ChangedByUserInfo changedByUserInfo,
          UserOrchestrator userOrchestrator)
        {
            // Arrange
            var controller = new UserController(userOrchestrator);

            mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId)))
                .ReturnsAsync((User)null)
                .Verifiable();

            // Act
            var response = await controller.Resume(userId, changedByUserInfo);

            // Assert
            mediator.Verify(x => x.SendAsync(It.IsAny<ResumeUserCommand>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task WhenSuspendedUserFound_ThenShouldResume(
         User user,
         string userId,
         DateTime lastSuspendedDate,
         [Frozen] Mock<IMediator> mediator,
         ChangedByUserInfo changedByUserInfo,
         UserOrchestrator userOrchestrator)
        {
            // Arrange
            user.IsLocked = false;
            user.IsActive = true;
            user.IsSuspended = true;
            user.LastSuspendedDate = lastSuspendedDate;
            var controller = new UserController(userOrchestrator);

            mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId)))
                .ReturnsAsync(user);

            // Act
            var response = await controller.Resume(userId, changedByUserInfo);

            // Assert
            mediator.Verify(x => x.SendAsync(It.Is<ResumeUserCommand>(c => c.User.Id == userId)), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task WhenLockedUserFound_ThenShouldNotResumeWithError(
         User user,
         string userId,
         [Frozen] Mock<IMediator> mediator,
         ChangedByUserInfo changedByUserInfo,
         UserOrchestrator userOrchestrator)
        {
            // Arrange
            user.IsLocked = true;
            user.IsActive = true;
            user.IsSuspended = false;
            var controller = new UserController(userOrchestrator);

            mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId)))
                .ReturnsAsync(user)
                .Verifiable();

            // Act
            var response = await controller.Resume(userId, changedByUserInfo) as OkNegotiatedContentResult<ResumeUserResponse>;

            // Assert
            mediator.Verify(x => x.SendAsync(It.Is<ResumeUserCommand>(c => c.User.Id == userId)), Times.Never);
            response.Content.Errors.Should().ContainKey("Active - only suspended accounts can be reinstated");
        }

        [Test, MoqAutoData]
        public async Task WhenActiveUsedFound_ThenShouldNotResumeWithError(
         User user,
         string userId,
         [Frozen] Mock<IMediator> mediator,
         ChangedByUserInfo changedByUserInfo,
         UserOrchestrator userOrchestrator)
        {
            // Arrange
            user.IsLocked = false;
            user.IsActive = true;
            user.IsSuspended = false;
            var controller = new UserController(userOrchestrator);

            mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId)))
                .ReturnsAsync(user)
                .Verifiable();

            // Act
            var response = await controller.Resume(userId, changedByUserInfo) as OkNegotiatedContentResult<ResumeUserResponse>;

            // Assert
            mediator.Verify(x => x.SendAsync(It.Is<ResumeUserCommand>(c => c.User.Id == userId)), Times.Never);
            response.Content.Errors.Should().ContainKey("Active - only suspended accounts can be reinstated");
        }
    }
}
