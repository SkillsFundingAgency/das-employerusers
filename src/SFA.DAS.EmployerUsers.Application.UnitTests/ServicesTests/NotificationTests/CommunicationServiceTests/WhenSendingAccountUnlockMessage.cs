using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.NotificationTests.CommunicationServiceTests
{
    public class WhenSendingAccountUnlockMessage
    {
        private const string MessageId = "MESSAGE_ID";

        private Mock<IHttpClientWrapper> _httpClient;
        private CommunicationService _communicationService;
        private User _user;

        [SetUp]
        public void Arrange()
        {
            _httpClient = new Mock<IHttpClientWrapper>();
            _httpClient.Setup(c => c.SendMessage(It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult<object>(null));

            _communicationService = new CommunicationService(_httpClient.Object);

            _user = new User
            {
                Id = "USER_ID",
                Email = "unit.tests@testing.local",
                UnlockCode = "UNLOCK_CODE"
            };
        }

        [Test]
        public async Task ThenItShouldSendHttpMessageWithCorrectProperties()
        {
            // Act
            await _communicationService.SendAccountLockedMessage(_user, MessageId);

            // Assert
            _httpClient.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.MessageType == "AccountLocked")), Times.Once);
            _httpClient.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.UserId == _user.Id)), Times.Once);
            _httpClient.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.RecipientsAddress == _user.Email)), Times.Once);
            _httpClient.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.ReplyToAddress == "info@sfa.das.gov.uk")), Times.Once);
            _httpClient.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.ForceFormat)), Times.Once);
            _httpClient.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.Data.ContainsKey("UnlockCode") && s.Data["UnlockCode"] == _user.UnlockCode)), Times.Once);
            _httpClient.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.Data.ContainsKey("MessageId") && s.Data["MessageId"] == MessageId)), Times.Once);
        }


        private Dictionary<string, string> ItIsDictionaryContaining(string key, string value)
        {
            return It.Is<Dictionary<string, string>>(d => d.ContainsKey(key) && d[key] == value);
        }
    }
}
