using System.Runtime.CompilerServices;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Application.UnitTests.Users.Handlers.Queries;

public class WhenHandlingGetUserByEmail
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Query_Is_Handled_And_User_Returned(
        Guid id,
        GetUserByEmailQuery request,
        UserProfileEntity user,
        [Frozen] Mock<IUserProfileRepository> repository,
        GetUserByEmailQueryHandler handler)
    {
        user.Id = id.ToString();
        repository.Setup(x => x.GetByEmail(request.Email)).ReturnsAsync(user);
        
        var actual = await handler.Handle(request, CancellationToken.None);

        actual.UserProfile.Should().BeEquivalentTo((UserProfile) user!);
    }
}