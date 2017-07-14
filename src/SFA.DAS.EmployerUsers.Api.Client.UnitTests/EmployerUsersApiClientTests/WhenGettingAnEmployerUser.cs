using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;

namespace SFA.DAS.EmployerUsers.Api.Client.UnitTests.EmployerUsersApiClientTests
{
    [TestFixture]
    public class WhenGettingAnEmployerUser : EmployerUsersApiClientTestsBase
    {
        [Test]
        public async Task ThenThenEmployerUserIsReturned()
        {
            var resourceUri = "api/users/ABC123";
            var expectedResult = new UserViewModel
            {
                FirstName = "Joe",
                LastName = "Bloggs",
                Id = "ABC123",
                Email = "test@email.com",
                IsActive = true,
                IsLocked = true,
                FailedLoginAttempts = 3
            };

            HttpClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(JsonConvert.SerializeObject(expectedResult));

            var response = await Client.GetResource<UserViewModel>(resourceUri);

            HttpClient.Verify(x => x.GetAsync(Configuration.ApiBaseUrl + resourceUri));
            response.ShouldBeEquivalentTo(expectedResult);
        }
    }
}
