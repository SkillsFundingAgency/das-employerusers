using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerProfiles.Api.Controllers;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByGovIdentifier;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserById;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Api.UnitTests.Controllers.Users;

public class WhenGettingUserByIdentifier
{
    [Test, MoqAutoData]
    public async Task Then_If_Found_The_Record_Is_Returned(
        string govIdentifier,
        GetUserByGovIdentifierQueryResult getUserResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        mediator.Setup(x => x.Send(It.Is<GetUserByGovIdentifierQuery>(c => c.GovIdentifier.Equals(govIdentifier)),
            CancellationToken.None)).ReturnsAsync(getUserResult);
        
        //Act
        var actual = await controller.GetUserById(govIdentifier);
        
        //Assert
        var result = actual as OkObjectResult;
        var actualResult = result.Value as UserProfile;
        actualResult.Should().BeEquivalentTo(getUserResult.UserProfile);
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_Id_Is_Guid_And_Found_The_Record_Is_Returned(
        Guid id,
        GetUserByIdQueryResult getUserResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        mediator.Setup(x => x.Send(It.Is<GetUserByIdQuery>(c => c.Id.Equals(id)),
            CancellationToken.None)).ReturnsAsync(getUserResult);
        
        //Act
        var actual = await controller.GetUserById(id.ToString());
        
        //Assert
        var result = actual as OkObjectResult;
        var actualResult = result.Value as UserProfile;
        actualResult.Should().BeEquivalentTo(getUserResult.UserProfile);
    }

    [Test, MoqAutoData]
    public async Task Then_If_Not_Found_For_Gov_Identifier_Then_NotFound_Result_Returned(
        string govIdentifier,
        GetUserByGovIdentifierQueryResult getUserResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        mediator.Setup(x => x.Send(It.Is<GetUserByGovIdentifierQuery>(c => c.GovIdentifier.Equals(govIdentifier)),
            CancellationToken.None)).ReturnsAsync(new GetUserByGovIdentifierQueryResult
        {
            UserProfile = null
        });
        
        //Act
        var actual = await controller.GetUserById(govIdentifier);
        
        //Assert
        actual.Should().BeAssignableTo<NotFoundResult>();
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_Not_Found_Id_Then_NotFound_Result_Returned(
        Guid id,
        GetUserByGovIdentifierQueryResult getUserResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        mediator.Setup(x => x.Send(It.Is<GetUserByIdQuery>(c => c.Id.Equals(id)),
            CancellationToken.None)).ReturnsAsync(new GetUserByIdQueryResult()
        {
            UserProfile = null
        });
        
        //Act
        var actual = await controller.GetUserById(id.ToString());
        
        //Assert
        actual.Should().BeAssignableTo<NotFoundResult>();
    }

    [Test, MoqAutoData]
    public async Task Then_If_Error_Then_InternalServerError_Response_Returned(
        string govIdentifier,
        GetUserByGovIdentifierQueryResult getUserResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        mediator.Setup(x => x.Send(It.Is<GetUserByGovIdentifierQuery>(c => c.GovIdentifier.Equals(govIdentifier)),
            CancellationToken.None)).ThrowsAsync(new Exception("Error"));
        
        //Act
        var actual = await controller.GetUserById(govIdentifier);
        
        //Assert
        var result = actual as StatusCodeResult;
        result?.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
    }
}