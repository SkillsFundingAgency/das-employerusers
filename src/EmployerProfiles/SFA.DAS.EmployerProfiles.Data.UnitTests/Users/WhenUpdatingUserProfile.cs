using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.EmployerProfiles.Data.UnitTests.DatabaseMock;
using SFA.DAS.EmployerProfiles.Data.Users;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Data.UnitTests.Users;

public class WhenUpdatingUserProfile
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_If_The_Record_Exists_Then_It_Is_Updated(
        string email,
        string govIdentifier,
        UserProfileEntity updateEntity,
        UserProfileEntity userProfileEntity,
        [Frozen] Mock<IEmployerProfilesDataContext> employerProfileDataContext,
        UserProfileRepository repository)
    {
        //Arrange
        userProfileEntity.Email = email;
        updateEntity.Id = Guid.NewGuid().ToString();
        updateEntity.Email = email;
        updateEntity.FirstName = null;
        updateEntity.GovUkIdentifier = govIdentifier;
        employerProfileDataContext.Setup(x => x.UserProfileEntities).ReturnsDbSet(new List<UserProfileEntity>{userProfileEntity});
        
        //Act
        var actual = await repository.Upsert(updateEntity);
        
        //Assert
        employerProfileDataContext.Verify(x => x.SaveChanges(), Times.Once);
        actual.Item1.Should().BeEquivalentTo(userProfileEntity, c => c.Excluding(o => o.GovUkIdentifier));
        userProfileEntity.FirstName.Should().Be(userProfileEntity.FirstName);
        userProfileEntity.GovUkIdentifier.Should().Be(govIdentifier);
        actual.Item2.Should().BeFalse();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_If_The_Record_Does_Not_Exist_Then_Inserted_And_Returned(
        UserProfileEntity entity,
        UserProfileEntity updateEntity,
        [Frozen] Mock<IEmployerProfilesDataContext> employerProfileDataContext,
        UserProfileRepository repository)
    {
        //Arrange
        updateEntity.Id = Guid.NewGuid().ToString();
        employerProfileDataContext.Setup(x => x.UserProfileEntities).ReturnsDbSet(new List<UserProfileEntity>{entity});
        
        //Act
        var actual = await repository.Upsert(updateEntity);
        
        //Assert
        employerProfileDataContext.Verify(x => x.SaveChanges(), Times.Once);
        actual.Item1.Should().BeEquivalentTo(updateEntity);
        actual.Item2.Should().BeTrue();
    }
}