using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;

namespace SFA.DAS.EmployerUsers.Api.Client.UnitTests.EmployerUsersApiClientTests
{
    [TestFixture]
    public class WhenSuspendingAUser : EmployerUsersApiClientTestsBase
    {
        [Test]
        public async Task ThenNoErrorsAreReturned_WhenTheRequestIsValid()
        {
            var userId = "ABC123";
            var resourceUri = $"api/users/{userId}/suspend";
            var expectedResult = new SuspendUserResponse
            {
                Id = userId,
                Errors = new Dictionary<string, string>()
            };

            HttpClient.Setup(x => x.PostAsync($"{Configuration.ApiBaseUrl}{resourceUri}", It.IsAny<HttpContent>()))
                .ReturnsAsync(JsonConvert.SerializeObject(expectedResult));

            var response = await Client.SuspendUser(userId);

            HttpClient.Verify(x => x.PostAsync($"{Configuration.ApiBaseUrl}{resourceUri}", It.IsAny<HttpContent>()), Times.Once);
            response.ShouldBeEquivalentTo(expectedResult);
        }
    }
}
