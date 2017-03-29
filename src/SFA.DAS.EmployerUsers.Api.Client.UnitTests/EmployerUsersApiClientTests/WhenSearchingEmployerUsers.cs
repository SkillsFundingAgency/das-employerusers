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
    public class WhenSearchingEmployerUsers : EmployerUsersApiClientTestsBase
    {
        [Test]
        public async Task ThenEmployerUsersAreReturned()
        {
            var pageNumber = 123;
            var pageSize = 9485;
            var criteria = "abc";
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
                        Href = "api/users/ABC123",
                        Email = "joe@blogs.com",
                        IsActive = true,
                        IsLocked = false
                    }
                }
            };

            HttpClient.Setup(x => x.GetAsync(Configuration.ApiBaseUrl + $"api/users/search/{criteria}/?pageNumber={pageNumber}&pageSize={pageSize}")).ReturnsAsync(JsonConvert.SerializeObject(expectedResult));

            var response = await Client.SearchEmployerUsers(criteria, pageNumber, pageSize);

            response.ShouldBeEquivalentTo(expectedResult);
        }
    }
}
