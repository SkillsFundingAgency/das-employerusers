using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.NotificationTests.CommunicationServiceTests
{
    public class WhenSendingUserAccountConfirmationMessage : CommunicationServiceTestBase
    {
        [SetUp]
        protected override void Arrange()
        {
            base.Arrange();

            ExpectedTemplateId = "UserAccountConfirmation";
            ExpectedSubject = "Welcome to your apprenticeship levy account";
        }

        protected override async Task Act()
        {
            await CommunicationService.SendUserAccountConfirmationMessage(User, MessageId);
        }
    }
}
