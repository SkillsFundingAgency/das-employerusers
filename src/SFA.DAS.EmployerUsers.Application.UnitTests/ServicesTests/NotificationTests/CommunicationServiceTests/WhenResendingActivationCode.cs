using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.NotificationTests.CommunicationServiceTests
{
    [TestFixture]
    public class WhenResendingActivationCode
    {
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private CommunicationService _communicationService;

        [SetUp]
        public void Setup()
        {
            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _communicationService = new CommunicationService(_httpClientWrapper.Object);
        }

        [Test]
        public async Task ThenItShouldSendAHttpMessageWithTheCorrectContent()
        {
            // Arrange
            var accessCode = "123456";
            var user = new User
            {
                Email = "test@test.com",
                Id = Guid.NewGuid().ToString(),
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = accessCode + "a",
                        CodeType = SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.Now.AddMinutes(9)
                    },
                    new SecurityCode
                    {
                        Code = accessCode,
                        CodeType = SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.Now.AddMinutes(10)
                    }
                }
            };
            var messageId = Guid.NewGuid().ToString();

            // Act
            await _communicationService.ResendActivationCodeMessage(user, messageId);

            // Assert
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.MessageType == "ResendActivationCode")), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.UserId == user.Id)), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.RecipientsAddress == user.Email)), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.ReplyToAddress == "info@sfa.das.gov.uk")), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.ForceFormat)), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.Data.ContainsKey("AccessCode") && s.Data["AccessCode"] == accessCode)), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.Data.ContainsKey("MessageId") && s.Data["MessageId"] == messageId)), Times.Once);
        }
    }
}