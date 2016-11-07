using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    [TestFixture]
    public class WhenResendingActivationCode : ControllerTestBase
    {
        private const string EmployerPortalUrl = "http://employerportal.local";
        private const string Action = "resend";
        private const string UserId = "myid";

        private Mock<AccountOrchestrator> _accountOrchestrator;
        private Mock<IConfigurationService> _configurationService;
        private AccountController _accountController;

        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            _httpContext.Setup(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(DasClaimTypes.Id, UserId),
            })));

            _accountOrchestrator = new Mock<AccountOrchestrator>();

            _configurationService = new Mock<IConfigurationService>();
            _configurationService.Setup(s => s.GetAsync<EmployerUsersConfiguration>())
                .Returns(Task.FromResult(new EmployerUsersConfiguration
                {
                    IdentityServer = new IdentityServerConfiguration
                    {
                        EmployerPortalUrl = EmployerPortalUrl
                    }
                }));

            _accountController = new AccountController(_accountOrchestrator.Object, null, _configurationService.Object)
            {
                ControllerContext = _controllerContext.Object
            };
        }

        [Test]
        public async Task ThenTheViewModelValuesArePassedToTheOrchestrator()
        {
            var accessCodeViewModel = new AccessCodeViewModel();

            var actual = await _accountController.Confirm(accessCodeViewModel, Action);

            _accountOrchestrator.Verify(x => x.ResendActivationCode(It.Is<ResendActivationCodeViewModel>(p => p.UserId.Equals(UserId))), Times.Once);

            var viewResult = actual as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.ViewName, Is.EqualTo("Confirm"));
        }

        //TODO: Create negative test case for this.
    }
}