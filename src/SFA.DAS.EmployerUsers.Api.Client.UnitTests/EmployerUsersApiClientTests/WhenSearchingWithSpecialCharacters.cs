using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;

namespace SFA.DAS.EmployerUsers.Api.Client.UnitTests.EmployerUsersApiClientTests
{
    public class WhenSearchingWithSpecialCharacters : EmployerUsersApiClientTestsBase
    {
        [Test]
        [TestCase("joe@blogs.co.uk", "joe%40blogs%2Eco%2Euk")]
        [TestCase("hammer & anvil", "hammer%20%26%20anvil")]
        public async Task ThenEmployerUsersAreReturned(string criteria, string encoded)
        {
            var pageNumber = 123;
            var pageSize = 9485;
            var expectedResult = new PagedApiResponseViewModel<UserSummaryViewModel>
            {
                Page = pageNumber,
                TotalPages = pageNumber + 2,
                Data = new List<UserSummaryViewModel>
                {
                    new UserSummaryViewModel
                    {
                        FirstName = "Joe",
                        LastName = "Bloggs",
                        Id = "ABC123",
                        Href = "api/users/ABC123"
                    }
                }
            };

            HttpClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(JsonConvert.SerializeObject(expectedResult));

            var response = await Client.SearchEmployerUsers(criteria, pageNumber, pageSize);

            // Assert
            HttpClient.Verify(x => x.GetAsync(Configuration.ApiBaseUrl + $"api/users/search/{encoded}/?pageNumber={pageNumber}&pageSize={pageSize}"));
            response.ShouldBeEquivalentTo(expectedResult);
        }
    }
}
