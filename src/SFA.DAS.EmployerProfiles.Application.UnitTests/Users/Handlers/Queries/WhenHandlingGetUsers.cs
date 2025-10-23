using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUsers;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Application.UnitTests.Users.Handlers.Queries;

public class WhenHandlingGetUsers
{
    [Test, MoqAutoData]
    public async Task Then_Returns_Users_From_Repository(
        GetUsersQuery query,
        List<UserProfileEntity> userProfileEntities,
        int totalCount,
        [Frozen] Mock<IUserProfileRepository> repository,
        GetUsersQueryHandler handler)
    {
        // Arrange
        foreach (var entity in userProfileEntities)
        {
            entity.Id = Guid.NewGuid().ToString();
        }
        
        var getAllUsersResult = new GetAllUsersResult
        {
            UserProfiles = userProfileEntities,
            TotalCount = totalCount
        };
        
        repository.Setup(x => x.GetAllUsers(query.PageSize, query.PageNumber))
            .ReturnsAsync(getAllUsersResult);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.UserProfiles.Should().BeEquivalentTo(userProfileEntities.Select(x => (UserProfile)x!));
        result.TotalCount.Should().Be(totalCount);
        result.PageNumber.Should().Be(query.PageNumber);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalPages.Should().Be((int)Math.Ceiling((double)totalCount / query.PageSize));
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_Empty_List_When_No_Users_Found(
        GetUsersQuery query,
        [Frozen] Mock<IUserProfileRepository> repository,
        GetUsersQueryHandler handler)
    {
        // Arrange
        var getAllUsersResult = new GetAllUsersResult
        {
            UserProfiles = new List<UserProfileEntity>(),
            TotalCount = 0
        };
        
        repository.Setup(x => x.GetAllUsers(query.PageSize, query.PageNumber))
            .ReturnsAsync(getAllUsersResult);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.UserProfiles.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }
}
