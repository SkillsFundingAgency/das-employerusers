using System.Web.Http.Routing;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Controllers;
using SFA.DAS.EmployerUsers.Api.Orchestrators;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.SearchControllerTests
{
    public abstract class SearchControllerTestsBase
    {
        protected SearchController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<ILogger> Logger;
        protected Mock<UrlHelper> UrlHelper;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILogger>();
            UrlHelper = new Mock<UrlHelper>();

            var orchestrator = new SearchOrchestrator(Mediator.Object, Logger.Object);
            Controller = new SearchController(orchestrator);
            Controller.Url = UrlHelper.Object;
        }
    }
}
