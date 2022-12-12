using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Domain.Tests.UserProfiles;

public class WhenConvertingFromEntity
{
    [Test, AutoData]
    public void Then_The_Fields_Are_Mapped(UserProfileEntity source)
    {
        var actual = (UserProfile) source;

        actual.Should().BeEquivalentTo(source);
        actual.DisplayName.Should().Be($"{source.FirstName} {source.LastName}");
    }
}