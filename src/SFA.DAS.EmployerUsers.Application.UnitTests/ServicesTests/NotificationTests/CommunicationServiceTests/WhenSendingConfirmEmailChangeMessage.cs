using System;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.NotificationTests.CommunicationServiceTests
{
    public class WhenSendingConfirmEmailChangeMessage
    {
        private const string UserId = "USER1";
        private const string NewEmailAddress = "user@unit.tests";
        private const string ConfirmationCode = "ABC123";
        public const string MessageId = "MESSAGE1";

        private User _user;
        private Mock<INotificationsApi> _notificationsApi;
        private CommunicationService _communicationService;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _user = new User
            {
                Id = UserId,
                Email = "user.one@unit.tests",
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = ConfirmationCode,
                        CodeType = SecurityCodeType.ConfirmEmailCode,
                        ExpiryTime = DateTime.MaxValue,
                        PendingValue = NewEmailAddress
                    }
                }
            };

            _notificationsApi = new Mock<INotificationsApi>();
            _logger = new Mock<ILogger>();

            _communicationService = new CommunicationService(_notificationsApi.Object, _logger.Object);
        }

        [Test]
        public async Task ThenItShouldSendAMessageOfTypeConfirmEmailChange()
        {
            // Act
            await _communicationService.SendConfirmEmailChangeMessage(_user, MessageId);

            // Assert
            _notificationsApi.Verify(c => c.SendEmail(It.Is<Email>(n => n.TemplateId == "ConfirmEmailChange")), Times.Once);
        }

        [Test]
        public async Task ThenItShouldSendAMessageToThePendingEmailAddress()
        {
            // Act
            await _communicationService.SendConfirmEmailChangeMessage(_user, MessageId);

            // Assert
            _notificationsApi.Verify(c => c.SendEmail(It.Is<Email>(n => n.RecipientsAddress == NewEmailAddress)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldSendAMessageThatShouldBeRepliedToSfa()
        {
            // Act
            await _communicationService.SendConfirmEmailChangeMessage(_user, MessageId);

            // Assert
            _notificationsApi.Verify(c => c.SendEmail(It.Is<Email>(n => n.ReplyToAddress == "info@sfa.das.gov.uk")), Times.Once);
        }

        [Test]
        public async Task ThenItShouldSendAMessageThatIncludesTheConfirmationCode()
        {
            // Act
            await _communicationService.SendConfirmEmailChangeMessage(_user, MessageId);

            // Assert
            _notificationsApi.Verify(c => c.SendEmail(It.Is<Email>(n => n.Tokens.ContainsKey("ConfirmEmailCode") && n.Tokens["ConfirmEmailCode"] == ConfirmationCode)), Times.Once);
        }

    }
}
