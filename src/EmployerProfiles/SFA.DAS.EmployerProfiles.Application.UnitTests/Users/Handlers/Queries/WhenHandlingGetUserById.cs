using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserById;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Application.UnitTests.Users.Handlers.Queries;

public class WhenHandlingGetUserById
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Query_Is_Handled_And_User_Returned(
        GetUserByIdQuery request,
        Guid id,
        UserProfileEntity user,
        [Frozen] Mock<IUserProfileRepository> repository,
        GetUserByIdQueryHandler handler)
    {
        user.Id = id.ToString();
        repository.Setup(x => x.GetById(request.Id)).ReturnsAsync(user);
        
        var actual = await handler.Handle(request, CancellationToken.None);

        actual.UserProfile.Should().BeEquivalentTo((UserProfile) user!);
    }
}