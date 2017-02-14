using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;

namespace SFA.DAS.EmployerUsers.Api.Client.UnitTests.EmployerUsersApiClientTests
{
    [TestFixture]
    public class WhenGettingAPageOfEmployerUsers : EmployerUsersApiClientTestsBase
    {
        [Test]
        public async Task ThenEmployerUsersAreReturned()
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

            HttpClient.Setup(x => x.GetAsync(Configuration.ApiBaseUrl + $"api/users?pageNumber={pageNumber}&pageSize={pageSize}")).ReturnsAsync(JsonConvert.SerializeObject(expectedResult));

            var response = await Client.GetPageOfEmployerUsers(pageNumber, pageSize);

            response.ShouldBeEquivalentTo(expectedResult);
        }
    }
}
