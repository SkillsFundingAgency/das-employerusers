using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.NotificationTests.CommunicationServiceTests
{
    public class WhenSendingPasswordResetConfirmationMessage : CommunicationServiceTestBase
    {
        [SetUp]
        protected override void Arrange()
        {
            base.Arrange();

            ExpectedTemplateId = "PasswordResetConfirmation";
            ExpectedSubject = "Your password has been reset";
        }

        protected override async Task Act()
        {
            await CommunicationService.SendPasswordResetConfirmationMessage(User, MessageId);
        }
    }
}
