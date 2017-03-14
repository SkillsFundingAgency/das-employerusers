using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.NotificationTests.CommunicationServiceTests
{
    public class WhenSendingPasswordResetCodeMessage : CommunicationServiceTestBase
    {
        private const string PasswordResetCode = "ResetCode";

        [SetUp]
        protected override void Arrange()
        {
            base.Arrange();

            User.SecurityCodes = new[]
            {
                new Domain.SecurityCode
                {
                    Code = PasswordResetCode,
                    CodeType = Domain.SecurityCodeType.PasswordResetCode,
                    ExpiryTime = DateTime.MaxValue
                }
            };

            ExpectedTemplateId = "PasswordReset";
            ExpectedSubject = "Reset Password: apprenticeship levy account";
        }

        [Test]
        public async Task ThenItShouldIncludePasswordResetCode()
        {
            await ThenItShouldIncludeCorrectTokenValue("Code", PasswordResetCode);
        }

        protected override async Task Act()
        {
            await CommunicationService.SendPasswordResetCodeMessage(User, MessageId, It.IsAny<string>());
        }
    }
}
