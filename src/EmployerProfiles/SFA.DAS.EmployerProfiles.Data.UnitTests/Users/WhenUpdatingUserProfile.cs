using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using SFA.DAS.EmployerProfiles.Data.UnitTests.DatabaseMock;
using SFA.DAS.EmployerProfiles.Data.Users;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;
using SFA.DAS.Testing.AutoFixture;
using System.Reflection.Metadata;

namespace SFA.DAS.EmployerProfiles.Data.UnitTests.Users;

public class WhenUpdatingUserProfile
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_If_The_Record_Exists_Then_It_Is_Updated(
        Guid id,
        string email,
        string govIdentifier,
        UserProfileEntity updateEntity,
        UserProfileEntity userProfileEntity,
        [Frozen] Mock<IEmployerProfilesDataContext> employerProfileDataContext,
        UserProfileRepository repository)
    {
        //Arrange
        userProfileEntity.Id = id.ToString();
        updateEntity.Id = id.ToString();
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

    [Test, RecursiveMoqAutoData]
    public void Throw_Exception_If_The_Id_Does_Not_Given(
        UserProfileEntity updateEntity,
        UserProfileRepository repository)
    {
        //Arrange
        updateEntity.Id = null;

        //Act
        var exception = Assert.ThrowsAsync<ArgumentNullException>(
            async () => await repository.Upsert(updateEntity),
            "Value cannot be null. (Parameter 'Id')");

        //Assert
        Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'Id')"));
    }

    [Test, RecursiveMoqAutoData]
    public void Throw_Exception_If_The_Email_Does_Not_Given(
        UserProfileEntity updateEntity,
        UserProfileRepository repository)
    {
        //Arrange
        updateEntity.Id = new Guid().ToString();
        updateEntity.Email = string.Empty;

        //Act
        var exception = Assert.ThrowsAsync<ArgumentNullException>(
            async () => await repository.Upsert(updateEntity),
            "Value cannot be null. (Parameter 'Email')");

        //Assert
        Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'Email')"));
    }
}