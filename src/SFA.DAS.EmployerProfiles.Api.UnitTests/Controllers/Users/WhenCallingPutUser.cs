using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerProfiles.Api.ApiRequests;
using SFA.DAS.EmployerProfiles.Api.Controllers;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Api.UnitTests.Controllers.Users;

public class WhenCallingPutUser
{
    [Test, MoqAutoData]
    public async Task Then_If_MediatorCall_Returns_Created_Then_Created_Result_Returned(
        Guid id,
        UserProfileRequest userProfileRequest,
        UpsertUserResult upsertUserResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        upsertUserResult.IsCreated = true;
        mediator.Setup(x => x.Send(It.Is<UpsertUserRequest>(c => 
                c.Id.Equals(id)
                && c.Email.Equals(userProfileRequest.Email)
                && c.FirstName.Equals(userProfileRequest.FirstName)
                && c.LastName.Equals(userProfileRequest.LastName)
                && c.GovUkIdentifier.Equals(userProfileRequest.GovIdentifier)
            ), CancellationToken.None))
            .ReturnsAsync(upsertUserResult);
        
        //Act
        var actual = await controller.PutUser(id, userProfileRequest);
        
        //Assert
        var result = actual as CreatedResult;
        var actualResult = result.Value as UserProfile;
        actualResult.Should().BeEquivalentTo(upsertUserResult.UserProfile);
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_MediatorCall_Returns_NotCreated_Then_Ok_Result_Returned(
        Guid id,
        UserProfileRequest userProfileRequest,
        UpsertUserResult upsertUserResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        upsertUserResult.IsCreated = false;
        mediator.Setup(x => x.Send(It.Is<UpsertUserRequest>(c => 
                c.Id.Equals(id)
                && c.Email.Equals(userProfileRequest.Email)
                && c.FirstName.Equals(userProfileRequest.FirstName)
                && c.LastName.Equals(userProfileRequest.LastName)
                && c.GovUkIdentifier.Equals(userProfileRequest.GovIdentifier)
                ), CancellationToken.None))
            .ReturnsAsync(upsertUserResult);
        
        //Act
        var actual = await controller.PutUser(id, userProfileRequest);
        
        //Assert
        var result = actual as OkObjectResult;
        var actualResult = result.Value as UserProfile;
        actualResult.Should().BeEquivalentTo(upsertUserResult.UserProfile);
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_ValidationError_Then_BadRequest_Response_Returned(
        Guid govIdentifier,
        UserProfileRequest userProfileRequest,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        mediator.Setup(x => x.Send(It.IsAny<UpsertUserRequest>(),
            CancellationToken.None)).ThrowsAsync(new ValidationException("Error"));
        
        //Act
        var actual = await controller.PutUser(govIdentifier, userProfileRequest);
        
        //Assert
        var result = actual as BadRequestResult;
        result?.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_Error_Then_InternalServerError_Response_Returned(
        Guid govIdentifier,
        UserProfileRequest userProfileRequest,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        mediator.Setup(x => x.Send(It.IsAny<UpsertUserRequest>(),
            CancellationToken.None)).ThrowsAsync(new Exception("Error"));
        
        //Act
        var actual = await controller.PutUser(govIdentifier, userProfileRequest);
        
        //Assert
        var result = actual as StatusCodeResult;
        result?.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
    }
}