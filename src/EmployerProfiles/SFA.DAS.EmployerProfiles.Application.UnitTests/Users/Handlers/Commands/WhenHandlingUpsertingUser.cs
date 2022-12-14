using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Application.UnitTests.Users.Handlers.Commands;

public class WhenHandlingUpsertingUser
{
    [Test, MoqAutoData]
    public async Task Then_The_Repository_Is_Called_To_Upsert_The_UserProfile(
        UserProfileEntity result,
        UpsertUserRequest request,
        [Frozen] Mock<IUserProfileRepository> userProfileRepository,
        UpsertUserRequestHandler handler)
    {
        //Arrange
        result.Id = request.Id.ToString();
        userProfileRepository.Setup(x => x.Upsert(
                It.Is<UserProfileEntity>(c => 
                    c.Email.Equals(request.Email)
                    && c.FirstName.Equals(request.FirstName)
                    && c.LastName.Equals(request.LastName)
                    && c.GovUkIdentifier.Equals(request.GovUkIdentifier)
                    && c.Id.Equals(request.Id.ToString())
                    )))
            .ReturnsAsync(result);
        
        //Act
        var actual = await handler.Handle(request, CancellationToken.None);
        
        //Assert
        actual.UserProfile.Should().BeEquivalentTo(result, options=>options.Excluding(c=>c.Id));
        actual.UserProfile.Id.Should().Be(request.Id);
    }
}