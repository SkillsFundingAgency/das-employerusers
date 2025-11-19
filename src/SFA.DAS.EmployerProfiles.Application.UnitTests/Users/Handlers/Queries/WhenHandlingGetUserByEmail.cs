using System.Runtime.CompilerServices;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Application.UnitTests.Users.Handlers.Queries;

public class WhenHandlingGetUserByEmail
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Query_Is_Handled_And_Users_Returned(
        Guid id,
        GetUsersByEmailQuery request,
        List<UserProfileEntity> users,
        [Frozen] Mock<IUserProfileRepository> repository,
        GetUsersByEmailQueryHandler handler)
    {
        users.ForEach(user => user.Id = Guid.NewGuid().ToString());
        repository.Setup(x => x.GetAllProfilesForEmailAddress(request.Email)).ReturnsAsync(users);
        
        var actual = await handler.Handle(request, CancellationToken.None);

        actual.UserProfiles.Should().BeEquivalentTo(users.Select(x=>(UserProfile)x!));
    }
}