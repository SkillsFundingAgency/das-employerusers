using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.EmployerProfiles.Data.UnitTests.DatabaseMock;
using SFA.DAS.EmployerProfiles.Data.Users;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerProfiles.Data.UnitTests.Users;

public class WhenUpdatingUserSuspendedFlag
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_If_The_Record_Exists_Then_It_Is_Updated(
        Guid id,
        UserProfileEntity userProfileEntity,
        [Frozen] Mock<IEmployerProfilesDataContext> employerProfileDataContext,
        UserProfileRepository repository)
    {
        //Arrange
        userProfileEntity.Id = id.ToString();
        userProfileEntity.IsSuspended = false;
        employerProfileDataContext.Setup(x => x.UserProfileEntities).ReturnsDbSet(new List<UserProfileEntity>{userProfileEntity});
        
        //Act
        var actual = await repository.UpdateUserSuspendedFlag(id, true);
        
        //Assert
        employerProfileDataContext.Verify(x => x.SaveChanges(), Times.Once);
        userProfileEntity.IsSuspended.Should().BeTrue();
        actual.Should().BeTrue();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_If_The_Record_Does_Not_Exist_Then_Null_Returned(
        UserProfileEntity entity,
        [Frozen] Mock<IEmployerProfilesDataContext> employerProfileDataContext,
        UserProfileRepository repository)
    {
        //Arrange
        employerProfileDataContext.Setup(x => x.UserProfileEntities).ReturnsDbSet(new List<UserProfileEntity>{entity});
        
        //Act
        var actual = await repository.UpdateUserSuspendedFlag(Guid.NewGuid(), true);
        
        //Assert
        employerProfileDataContext.Verify(x => x.SaveChanges(), Times.Never);
        actual.Should().BeFalse();
    }
}