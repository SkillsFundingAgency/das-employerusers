using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.NotificationTests.CommunicationServiceTests
{
    public class WhenSendingUnlockedMessage : CommunicationServiceTestBase
    {
        [SetUp]
        protected override void Arrange()
        {
            base.Arrange();

            ExpectedTemplateId = "AccountUnLocked";
            ExpectedSubject = "Your account had been unlocked";
        }

        protected override async Task Act()
        {
            await CommunicationService.SendUserUnlockedMessage(User, MessageId);
        }
    }
}
