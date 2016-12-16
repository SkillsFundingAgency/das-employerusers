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
    public class WhenSendingAccountUnlockMessage
    {
        private const string MessageId = "MESSAGE_ID";
        private const string UnlockCode = "UNLOCK_CODE";
        private const string ReturnUrl = "http://myurl";

        private Mock<INotificationsApi> _notificationsApi;
        private CommunicationService _communicationService;
        private User _user;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _notificationsApi = new Mock<INotificationsApi>();
            _logger = new Mock<ILogger>();

            _communicationService = new CommunicationService(_notificationsApi.Object, _logger.Object);

            _user = new User
            {
                Id = "USER_ID",
                Email = "unit.tests@testing.local",
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = UnlockCode,
                        CodeType = SecurityCodeType.UnlockCode,
                        ExpiryTime = DateTime.MaxValue,
                        ReturnUrl = ReturnUrl
                    }
                }
            };
        }

        [Test]
        public async Task ThenItShouldSendHttpMessageWithCorrectProperties()
        {
            // Act
            await _communicationService.SendAccountLockedMessage(_user, MessageId);

            // Assert
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.TemplateId == "AccountLocked")), Times.Once);
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.RecipientsAddress == _user.Email)), Times.Once);
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.ReplyToAddress == "info@sfa.das.gov.uk")), Times.Once);
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.Tokens.ContainsKey("UnlockCode") && s.Tokens["UnlockCode"] == UnlockCode)), Times.Once);
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.Tokens.ContainsKey("ReturnUrl") && s.Tokens["ReturnUrl"] == ReturnUrl)), Times.Once);
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.Tokens.ContainsKey("CodeExpiry") && s.Tokens["CodeExpiry"].Equals(DateTime.MaxValue.ToString("d MMMM yyyy")))), Times.Once);
        }
        
    }
}
