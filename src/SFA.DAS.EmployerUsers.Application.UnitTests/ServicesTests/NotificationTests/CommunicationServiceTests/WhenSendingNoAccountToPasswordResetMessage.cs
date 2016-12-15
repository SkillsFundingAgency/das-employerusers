using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.NotificationTests.CommunicationServiceTests
{
    public class WhenSendingNoAccountToPasswordResetMessage : CommunicationServiceTestBase
    {
        [SetUp]
        protected override void Arrange()
        {
            base.Arrange();

            ExpectedTemplateId = "ForgottenPasswordNoAccount";
            ExpectedSubject = "Reset Password: apprenticeship levy account";
        }

        [Test]
        public async Task ThenItShouldIncludeRegisterUrl()
        {
            await ThenItShouldIncludeCorrectTokenValue("RegisterUrl", "some-url");
        }

        protected override async Task Act()
        {
            await CommunicationService.SendNoAccountToPasswordResetMessage(Email, MessageId, "some-url");
        }
    }
}
