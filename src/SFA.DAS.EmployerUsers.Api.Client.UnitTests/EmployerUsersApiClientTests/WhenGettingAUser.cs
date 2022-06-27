using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;

namespace SFA.DAS.EmployerUsers.Api.Client.UnitTests.EmployerUsersApiClientTests
{
    [TestFixture]
    public class WhenGettingAUser : EmployerUsersApiClientTestsBase
    {
        [Test, AutoData]
        public async Task ThenNoErrorsAreReturned_WhenTheRequestIsValid(string userId, UserViewModel result)
        {
            var resourceUri = $"api/users/{userId}";

            HttpClient.Setup(x => x.GetAsync($"{Configuration.ApiBaseUrl}{resourceUri}"))
                .ReturnsAsync(JsonConvert.SerializeObject(result));

            var response = await Client.GetUserById(userId);

            HttpClient.Verify(x => x.GetAsync($"{Configuration.ApiBaseUrl}{resourceUri}"), Times.Once);
            response.ShouldBeEquivalentTo(result);
        }
    }
}
