using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpdateUserSuspended;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Application.UnitTests.Users.Handlers.Commands;

public class WhenHandlingUpdatingUserSuspendedFlag
{
    [Test, MoqAutoData]
    public async Task Then_The_Repository_Is_Called_To_Update_The_Suspended_Flag(
        UpdateUserSuspendedRequest request,
        [Frozen] Mock<IUserProfileRepository> userProfileRepository,
        UpdateUserSuspendedRequestHandler handler)
    {
        //Arrange
        userProfileRepository.Setup(x => x.UpdateUserSuspendedFlag(request.Id, request.UserSuspended))
            .ReturnsAsync(true);
        
        //Act
        var actual = await handler.Handle(request, CancellationToken.None);
        
        //Assert
        actual.Updated.Should().BeTrue();
    }
}