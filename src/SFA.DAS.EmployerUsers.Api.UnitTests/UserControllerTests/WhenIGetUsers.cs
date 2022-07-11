using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Common;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Controllers;
using SFA.DAS.EmployerUsers.Api.DependencyResolution;
using SFA.DAS.EmployerUsers.Api.Orchestrators;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Queries.GetUsers;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.TestCommon.Extensions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.UserControllerTests
{
    [TestFixture]
    public class WhenIGetUsers
    {
        [Test, MoqAutoData]
        public async Task ThenAPageOfUsersIsReturned(
          GetUsersQueryResponse queryResponse,
          [Frozen] Mock<ILogger> loggerMock,
          [Frozen] Mock<IMediator> mediator,
          [Frozen] Mock<UrlHelper> urlHelperMock)
        {
            // Arrange
            var mapperReal = new MapperConfiguration(c => c.AddProfile<DefaultProfile>()).CreateMapper();

            var pageSize = 500;
            var pageNumber = 1;
            queryResponse.RecordCount = 3;
            var orchestrator = new UserOrchestrator(mapperReal, mediator.Object, loggerMock.Object);
            var controller = new UserController(orchestrator);
            controller.Url = urlHelperMock.Object;
            mediator.Setup(x => x.SendAsync(It.Is<GetUsersQuery>(q => q.PageNumber == pageNumber && q.PageSize == pageSize))).ReturnsAsync(queryResponse);            

            queryResponse.Users
                .ToList()
                .ForEach(x => urlHelperMock.Setup(y => y.Route("Show", It.Is<object>(o => o.IsEquivalentTo(new { x.Id })))).Returns($"/api/users/{x.Id}"));

            // Act
            var response = await controller.Index(pageSize, pageNumber);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PagedApiResponseViewModel<UserSummaryViewModel>>>(response);
            var model = response as OkNegotiatedContentResult<PagedApiResponseViewModel<UserSummaryViewModel>>;

            model?.Content.Should().NotBeNull();
            model.Content.Page.Should().Be(pageNumber);
            model.Content.Data.Should().HaveCount(queryResponse.RecordCount);

            for (var i = 0; i < queryResponse.Users.Length; i++)
            {
                model.Content.Data[i].ShouldBeEquivalentTo(queryResponse.Users[i], options => options.ExcludingMissingMembers());
                model.Content.Data[i].Href.Should().Be($"/api/users/{queryResponse.Users[i].Id}");
            }

            loggerMock.Verify(x => x.Info("Getting all user accounts."), Times.Once);
        }
    }
}
