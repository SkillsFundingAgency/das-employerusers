using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.EmployerProfiles.Api.Controllers;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUsers;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Api.UnitTests.Controllers.Users;

public class WhenGettingUsers
{
    [Test, MoqAutoData]
    public async Task Then_If_Users_Found_The_Records_Are_Returned(
        GetUsersQueryResult getResult,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        var pageSize = 100;
        var pageNumber = 1;
        mediator.Setup(x => x.Send(It.Is<GetUsersQuery>(c => c.PageSize == pageSize && c.PageNumber == pageNumber),
            CancellationToken.None)).ReturnsAsync(getResult);

        //Act
        var actual = await controller.GetUsers(pageSize: pageSize, pageNumber: pageNumber);
        
        //Assert
        var result = actual as OkObjectResult;
        var actualResult = result.Value as GetUsersQueryResult;
        actualResult.Should().BeEquivalentTo(getResult);
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_No_Users_Found_Then_EmptyList_Returned(
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        var pageSize = 100;
        var pageNumber = 1;
        mediator.Setup(x => x.Send(It.Is<GetUsersQuery>(c => c.PageSize == pageSize && c.PageNumber == pageNumber),
            CancellationToken.None)).ReturnsAsync(new GetUsersQueryResult
        {
            UserProfiles = new List<UserProfile>(),
            TotalCount = 0,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        
        //Act
        var actual = await controller.GetUsers(pageSize: pageSize, pageNumber: pageNumber);

        //Assert
        var result = actual as OkObjectResult;
        var actualResult = result.Value as GetUsersQueryResult;
        actualResult.UserProfiles.Count.Should().Be(0);
        actualResult.TotalCount.Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task Then_If_Error_Then_InternalServerError_Response_Returned(
        [Frozen] Mock<IMediator> mediator,
        [Greedy] UsersController controller)
    {
        //Arrange
        var pageSize = 100;
        var pageNumber = 1;
        mediator.Setup(x => x.Send(It.Is<GetUsersQuery>(c => c.PageSize == pageSize && c.PageNumber == pageNumber),
            CancellationToken.None)).ThrowsAsync(new Exception("Error"));
        
        //Act
        var actual = await controller.GetUsers(pageSize: pageSize, pageNumber: pageNumber);
        
        //Assert
        var result = actual as StatusCodeResult;
        result?.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
    }
}
