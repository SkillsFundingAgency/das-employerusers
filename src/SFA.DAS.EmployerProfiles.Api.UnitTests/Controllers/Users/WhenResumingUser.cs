using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.EmployerProfiles.Api.ApiRequests;
using SFA.DAS.EmployerProfiles.Api.ApiResponses;
using SFA.DAS.EmployerProfiles.Api.Controllers;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpdateUserSuspended;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByGovIdentifier;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserById;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Api.UnitTests.Controllers.Users;

public class WhenResumingUser
{
    [Test, MoqAutoData]
    public async Task Then_User_Can_Be_Resumed_By_Guid(
        Guid userId,
        ChangeUserStatusRequest request,
        UpdateUserSuspendedResult updateResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        // Arrange
        request.ChangedByEmail = "resume@test.com";
        request.ChangedByUserId = "actor";
        var userProfile = CreateUserProfile(userId, isSuspended: true);
        updateResult.Updated = true;

        mediator.Setup(x =>
                x.Send(It.Is<GetUserByIdQuery>(c => c.Id == userId), CancellationToken.None))
            .ReturnsAsync(new GetUserByIdQueryResult { UserProfile = userProfile });
        mediator.Setup(x =>
                x.Send(It.Is<UpdateUserSuspendedRequest>(c => c.Id == userId && !c.UserSuspended),
                    CancellationToken.None))
            .ReturnsAsync(updateResult);

        // Act
        var actual = await controller.ResumeUser(userId.ToString(), request);

        // Assert
        var result = actual as OkObjectResult;
        result.Should().NotBeNull();
        ((ChangeUserStatusResponse)result!.Value!).Id.Should().Be(userId.ToString());
    }

    [Test, MoqAutoData]
    public async Task Then_User_Can_Be_Resumed_By_GovIdentifier(
        string govIdentifier,
        Guid userId,
        ChangeUserStatusRequest request,
        UpdateUserSuspendedResult updateResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        // Arrange
        request.ChangedByEmail = "resume@test.com";
        request.ChangedByUserId = "actor";
        var userProfile = CreateUserProfile(userId, isSuspended: true, govIdentifier: govIdentifier);
        updateResult.Updated = true;

        mediator.Setup(x =>
                x.Send(It.Is<GetUserByGovIdentifierQuery>(c => c.GovIdentifier == govIdentifier),
                    CancellationToken.None))
            .ReturnsAsync(new GetUserByGovIdentifierQueryResult { UserProfile = userProfile });
        mediator.Setup(x =>
                x.Send(It.Is<UpdateUserSuspendedRequest>(c => c.Id == userId && !c.UserSuspended),
                    CancellationToken.None))
            .ReturnsAsync(updateResult);

        // Act
        var actual = await controller.ResumeUser(govIdentifier, request);

        // Assert
        var result = actual as OkObjectResult;
        result.Should().NotBeNull();
        ((ChangeUserStatusResponse)result!.Value!).Id.Should().Be(userId.ToString());
    }

    [Test, MoqAutoData]
    public async Task Then_BadRequest_Returned_When_User_Not_Suspended(
        Guid userId,
        ChangeUserStatusRequest request,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        // Arrange
        request.ChangedByEmail = "resume@test.com";
        request.ChangedByUserId = "actor";
        var userProfile = CreateUserProfile(userId, isSuspended: false);

        mediator.Setup(x =>
                x.Send(It.Is<GetUserByIdQuery>(c => c.Id == userId), CancellationToken.None))
            .ReturnsAsync(new GetUserByIdQueryResult { UserProfile = userProfile });

        // Act
        var actual = await controller.ResumeUser(userId.ToString(), request);

        // Assert
        var result = actual as BadRequestObjectResult;
        result.Should().NotBeNull();
        var response = result!.Value as ChangeUserStatusResponse;
        response!.Errors.Should().ContainKey("Active - only suspended accounts can be reinstated");
        mediator.Verify(x => x.Send(It.IsAny<UpdateUserSuspendedRequest>(), CancellationToken.None),
            Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_NotFound_Returned_When_User_Cannot_Be_Located(
        Guid userId,
        ChangeUserStatusRequest request,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        mediator.Setup(x =>
                x.Send(It.Is<GetUserByIdQuery>(c => c.Id == userId), CancellationToken.None))
            .ReturnsAsync(new GetUserByIdQueryResult());

        var actual = await controller.ResumeUser(userId.ToString(), request);

        actual.Should().BeOfType<NotFoundResult>();
    }

    [Test, MoqAutoData]
    public async Task Then_InternalServerError_Returned_When_Command_Fails(
        Guid userId,
        ChangeUserStatusRequest request,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        var userProfile = CreateUserProfile(userId, isSuspended: true);
        mediator.Setup(x =>
                x.Send(It.Is<GetUserByIdQuery>(c => c.Id == userId), CancellationToken.None))
            .ReturnsAsync(new GetUserByIdQueryResult { UserProfile = userProfile });
        mediator.Setup(x =>
                x.Send(It.IsAny<UpdateUserSuspendedRequest>(), CancellationToken.None))
            .ThrowsAsync(new Exception("Error"));

        var actual = await controller.ResumeUser(userId.ToString(), request);

        var result = actual as StatusCodeResult;
        result?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    private static UserProfile CreateUserProfile(Guid id, bool isSuspended, string? govIdentifier = null)
    {
        return new UserProfile
        {
            Id = id,
            Email = "user@test.com",
            FirstName = "Test",
            LastName = "User",
            DisplayName = "Test User",
            GovUkIdentifier = govIdentifier ?? Guid.NewGuid().ToString(),
            IsSuspended = isSuspended,
            IsActive = !isSuspended,
            IsLocked = false
        };
    }
}
