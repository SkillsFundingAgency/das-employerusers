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
            _httpClient.Verify(c => c.SendMessage(ItIsDictionaryContaining("body", _user.UnlockCode)), Times.Once());
            _httpClient.Verify(c => c.SendMessage(ItIsDictionaryContaining("UserId", _user.Id)), Times.Once());
            _httpClient.Verify(c => c.SendMessage(ItIsDictionaryContaining("MessageId", MessageId)), Times.Once());
            _httpClient.Verify(c => c.SendMessage(ItIsDictionaryContaining("messagetype", "SendEmail")), Times.Once());
            _httpClient.Verify(c => c.SendMessage(ItIsDictionaryContaining("toEmail", _user.Email)), Times.Once());
            _httpClient.Verify(c => c.SendMessage(ItIsDictionaryContaining("fromEmail", "info@sfa.das.gov.uk")), Times.Once());
        }


        private Dictionary<string, string> ItIsDictionaryContaining(string key, string value)
        {
            return It.Is<Dictionary<string, string>>(d => d.ContainsKey(key) && d[key] == value);
        }
    }
}
