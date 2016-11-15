using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.NotificationTests.CommunicationServiceTests
{
    public class WhenSendingConfirmEmailChangeMessage
    {
        private const string UserId = "USER1";
        private const string NewEmailAddress = "user@unit.tests";
        private const string ConfirmationCode = "ABC123";
        public const string MessageId = "MESSAGE1";

        private User _user;
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private CommunicationService _communicationService;

        [SetUp]
        public void Arrange()
        {
            _user = new User
            {
                Id = UserId,
                Email = "user.one@unit.tests",
                PendingEmail = NewEmailAddress,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = ConfirmationCode,
                        CodeType = SecurityCodeType.ConfirmEmailCode,
                        ExpiryTime = DateTime.MaxValue
                    }
                }
            };
            _httpClientWrapper = new Mock<IHttpClientWrapper>();

            _communicationService = new CommunicationService(_httpClientWrapper.Object);
        }

        [Test]
        public async Task ThenItShouldSendAMessageOfTypeConfirmEmailChange()
        {
            // Act
            await _communicationService.SendConfirmEmailChangeMessage(_user, MessageId);

            // Assert
            _httpClientWrapper.Verify(c => c.SendMessage(It.Is<EmailNotification>(n => n.MessageType == "ConfirmEmailChange")), Times.Once);
        }

        [Test]
        public async Task ThenItShouldSendAMessageWithTheUserId()
        {
            // Act
            await _communicationService.SendConfirmEmailChangeMessage(_user, MessageId);

            // Assert
            _httpClientWrapper.Verify(c => c.SendMessage(It.Is<EmailNotification>(n => n.UserId == UserId)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldSendAMessageToThePendingEmailAddress()
        {
            // Act
            await _communicationService.SendConfirmEmailChangeMessage(_user, MessageId);

            // Assert
            _httpClientWrapper.Verify(c => c.SendMessage(It.Is<EmailNotification>(n => n.RecipientsAddress == NewEmailAddress)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldSendAMessageThatShouldBeRepliedToSfa()
        {
            // Act
            await _communicationService.SendConfirmEmailChangeMessage(_user, MessageId);

            // Assert
            _httpClientWrapper.Verify(c => c.SendMessage(It.Is<EmailNotification>(n => n.ReplyToAddress == "info@sfa.das.gov.uk")), Times.Once);
        }

        [Test]
        public async Task ThenItShouldSendAMessageThatIncludesTheMessageId()
        {
            // Act
            await _communicationService.SendConfirmEmailChangeMessage(_user, MessageId);

            // Assert
            _httpClientWrapper.Verify(c => c.SendMessage(It.Is<EmailNotification>(n => n.Data.ContainsKey("MessageId") && n.Data["MessageId"] == MessageId)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldSendAMessageThatIncludesTheConfirmationCode()
        {
            // Act
            await _communicationService.SendConfirmEmailChangeMessage(_user, MessageId);

            // Assert
            _httpClientWrapper.Verify(c => c.SendMessage(It.Is<EmailNotification>(n => n.Data.ContainsKey("ConfirmEmailCode") && n.Data["ConfirmEmailCode"] == ConfirmationCode)), Times.Once);
        }

    }
}
