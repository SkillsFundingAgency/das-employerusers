using System;
using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.EmployerProfiles.Api.Controllers;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpdateUserSuspended;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Api.UnitTests.Controllers.Users;

public class WhenSuspendingUser
{
    [Test, MoqAutoData]
    public async Task Then_If_User_Found_And_Suspended_Success_Response_Returned(
        Guid userId,
        UpdateUserSuspendedResult updateResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        updateResult.Updated = true;
        mediator.Setup(x => x.Send(It.Is<UpdateUserSuspendedRequest>(c => c.Id == userId && c.UserSuspended == true),
            CancellationToken.None)).ReturnsAsync(updateResult);

        //Act
        var actual = await controller.SuspendUser(userId);
        
        //Assert
        var result = actual as OkObjectResult;
        result.Should().NotBeNull();
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_User_Not_Found_Then_NotFound_Returned(
        Guid userId,
        UpdateUserSuspendedResult updateResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        updateResult.Updated = false;
        mediator.Setup(x => x.Send(It.Is<UpdateUserSuspendedRequest>(c => c.Id == userId && c.UserSuspended == true),
            CancellationToken.None)).ReturnsAsync(updateResult);
        
        //Act
        var actual = await controller.SuspendUser(userId);

        //Assert
        var result = actual as NotFoundResult;
        result.Should().NotBeNull();
    }

    [Test, MoqAutoData]
    public async Task Then_If_Error_Then_InternalServerError_Response_Returned(
        Guid userId,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        mediator.Setup(x => x.Send(It.Is<UpdateUserSuspendedRequest>(c => c.Id == userId && c.UserSuspended == true),
            CancellationToken.None)).ThrowsAsync(new Exception("Error"));
        
        //Act
        var actual = await controller.SuspendUser(userId);
        
        //Assert
        var result = actual as StatusCodeResult;
        result?.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
    }
}
