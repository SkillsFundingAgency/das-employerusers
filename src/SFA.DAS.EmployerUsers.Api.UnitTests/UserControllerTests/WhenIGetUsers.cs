using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Queries.GetUsers;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.UserControllerTests
{
    [TestFixture]
    public class WhenIGetUsers : UserControllerTestsBase
    {
        [Test]
        public async Task ThenAPageOfUsersIsReturned()
        {
            var pageSize = 500;
            var pageNumber = 1;

            var users = new List<User>
            {
                new User { FirstName = "Joe", LastName = "Bloggs", Id = "1" },
                new User { FirstName = "John", LastName = "Smith", Id = "2" },
                new User { FirstName = "Jane", LastName = "Doe", Id = "3" }
            };

            var usersResponse = new GetUsersQueryResponse
            {
                RecordCount = 3,
                Users = users.ToArray()
            };
            Mediator.Setup(x => x.SendAsync(It.Is<GetUsersQuery>(q => q.PageNumber == pageNumber && q.PageSize == pageSize))).ReturnsAsync(usersResponse);
            users.ForEach(x => UrlHelper.Setup(y => y.Route("Show", It.Is<object>(o => o.GetHashCode() == new { x.Id }.GetHashCode()))).Returns($"/api/users/{x.Id}"));

            var response = await Controller.Index(pageSize, pageNumber);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PagedApiResponseViewModel<UserSummaryViewModel>>>(response);
            var model = response as OkNegotiatedContentResult<PagedApiResponseViewModel<UserSummaryViewModel>>;

            model?.Content.Should().NotBeNull();
            model.Content.Page.Should().Be(pageNumber);
            model.Content.Data.Should().HaveCount(usersResponse.RecordCount);

            for (var i = 0; i < users.Count; i++)
            {
                model.Content.Data[i].ShouldBeEquivalentTo(users[i], options => options.ExcludingMissingMembers());
                model.Content.Data[i].Href.Should().Be($"/api/users/{users[i].Id}");
            }

            Logger.Verify(x => x.Info("Getting all user accounts."), Times.Once);
        }
    }
}
