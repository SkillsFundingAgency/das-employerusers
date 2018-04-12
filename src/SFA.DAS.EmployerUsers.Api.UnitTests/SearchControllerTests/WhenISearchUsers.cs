using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Queries.SearchUsers;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.TestCommon.Extensions;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.SearchControllerTests
{
    [TestFixture]
    public class WhenISearchUsers : SearchControllerTestsBase
    {
        [Test]
        public async Task ThenAPageOfMatchingUsersIsReturned()
        {
            var pageSize = 500;
            var pageNumber = 1;
            var criteria = "abc";

            var users = new List<User>
            {
                new User { FirstName = "Joe", LastName = "Bloggs", Id = "1" },
                new User { FirstName = "John", LastName = "Smith", Id = "2" },
                new User { FirstName = "Jane", LastName = "Doe", Id = "3" }
            };

            var usersResponse = new SearchUsersQueryResponse
            {
                Users = users.ToArray(),
                RecordCount = 1400
            };
            Mediator.Setup(x => x.SendAsync(It.Is<SearchUsersQuery>(q => q.Criteria == criteria && q.PageNumber == pageNumber && q.PageSize == pageSize))).ReturnsAsync(usersResponse);
            users.ForEach(x => UrlHelper.Setup(y => y.Route("Show", It.Is<object>(o => o.IsEquivalentTo(new { x.Id })))).Returns($"/api/users/{x.Id}"));

            var response = await Controller.Search(criteria, pageSize, pageNumber);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PagedApiResponseViewModel<UserSummaryViewModel>>>(response);
            var model = response as OkNegotiatedContentResult<PagedApiResponseViewModel<UserSummaryViewModel>>;

            model?.Content.Should().NotBeNull();
            model.Content.Page.Should().Be(pageNumber);
            model.Content.TotalPages.Should().Be(3);
            model.Content.Data.Should().HaveCount(users.Count);

            for (var i = 0; i < users.Count; i++)
            {
                model.Content.Data[i].ShouldBeEquivalentTo(users[i], options => options.ExcludingMissingMembers());
                model.Content.Data[i].Href.Should().Be($"/api/users/{users[i].Id}");
            }

            Logger.Verify(x => x.Info($"Searching for user accounts using criteria {criteria}."), Times.Once);
        }
    }
}
