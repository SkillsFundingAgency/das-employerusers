using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.EmployerProfiles.Data.UnitTests.DatabaseMock;
using SFA.DAS.EmployerProfiles.Data.Users;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Data.UnitTests.Users;

public class WhenGettingUserProfile
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Item_Is_Returned_By_Email(
        List<UserProfileEntity> userProfiles,
        UserProfileEntity searchEntity,
        [Frozen] Mock<IEmployerProfilesDataContext> employerProfileDataContext,
        UserProfileRepository userProfileRepository)
    {
        userProfiles.Add(searchEntity);
        employerProfileDataContext.Setup(x => x.UserProfileEntities).ReturnsDbSet(userProfiles);
        
        var actual = await userProfileRepository.GetByEmail(searchEntity.Email);

        actual.Should().BeEquivalentTo(searchEntity);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Item_Is_Returned_By_Id(
        Guid id,
        List<UserProfileEntity> userProfiles,
        UserProfileEntity searchEntity,
        [Frozen] Mock<IEmployerProfilesDataContext> employerProfileDataContext,
        UserProfileRepository userProfileRepository)
    {
        searchEntity.Id = id.ToString();
        userProfiles.Add(searchEntity);
        employerProfileDataContext.Setup(x => x.UserProfileEntities).ReturnsDbSet(userProfiles);
        
        var actual = await userProfileRepository.GetById(Guid.Parse(searchEntity.Id));
        
        actual.Should().BeEquivalentTo(searchEntity);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Item_Is_Returned_By_GovIdentifier(
        List<UserProfileEntity> userProfiles,
        UserProfileEntity searchEntity,
        [Frozen] Mock<IEmployerProfilesDataContext> employerProfileDataContext,
        UserProfileRepository userProfileRepository)
    {
        userProfiles.Add(searchEntity);
        employerProfileDataContext.Setup(x => x.UserProfileEntities).ReturnsDbSet(userProfiles);

        var actual = await userProfileRepository.GetByGovIdentifier(searchEntity.GovUkIdentifier);
        
        actual.Should().BeEquivalentTo(searchEntity);
        
        
    }
}