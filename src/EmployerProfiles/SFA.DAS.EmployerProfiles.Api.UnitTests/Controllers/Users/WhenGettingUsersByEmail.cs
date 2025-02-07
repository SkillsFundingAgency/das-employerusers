using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.EmployerProfiles.Api.Controllers;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Api.UnitTests.Controllers.Users;

public class WhenGettingUsersByEmail
{
    [Test, MoqAutoData]
    public async Task Then_If_Email_Found_The_Record_Is_Returned(
        string email,
        GetUserByEmailQueryResult getResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        mediator.Setup(x => x.Send(It.Is<GetUserByEmailQuery>(c => c.Email.Equals(email)),
            CancellationToken.None)).ReturnsAsync(getResult);

        var expected = new List<UserProfile>();
        expected.Add(getResult.UserProfile);

        //Act
        var actual = await controller.GetUsersByQuery(email : email);
        
        //Assert
        var result = actual as OkObjectResult;
        var actualResult = result.Value as List<UserProfile>;
        actualResult.Should().BeEquivalentTo(expected);
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_Email_Not_Found_Then_EmptyList_Returned(
        string email,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        mediator.Setup(x => x.Send(It.Is<GetUserByEmailQuery>(c => c.Email.Equals(email)),
            CancellationToken.None)).ReturnsAsync(new GetUserByEmailQueryResult
            {
            UserProfile = null
        });
        
        //Act
        var actual = await controller.GetUsersByQuery(email: email);

        //Assert
        var result = actual as OkObjectResult;
        var actualResult = result.Value as List<UserProfile>;
        actualResult.Count.Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task Then_If_Error_Then_InternalServerError_Response_Returned(
        string email,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        mediator.Setup(x => x.Send(It.Is<GetUserByEmailQuery>(c => c.Email.Equals(email)),
            CancellationToken.None)).ThrowsAsync(new Exception("Error"));
        
        //Act
        var actual = await controller.GetUserById(email);
        
        //Assert
        var result = actual as StatusCodeResult;
        result?.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
    }
}