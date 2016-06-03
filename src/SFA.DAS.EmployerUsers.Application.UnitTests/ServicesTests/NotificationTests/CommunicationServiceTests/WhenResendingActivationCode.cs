using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
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
            var user = new User
            {
                AccessCode = "123456",
                Email = "test@test.com",
                Id = Guid.NewGuid().ToString(),
            };
            var messageId = Guid.NewGuid().ToString();

            //Act
            await _communicationService.ResendActivationCodeMessage(user, messageId);

            //Assert
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.MessageType == "ResendActivationCode")), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.UserId == user.Id)), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.RecipientsAddress == user.Email)), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.ReplyToAddress == "info@sfa.das.gov.uk")), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.ForceFormat)), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.Data.ContainsKey("AccessCode") && s.Data["AccessCode"] == user.AccessCode)), Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<EmailNotification>(s => s.Data.ContainsKey("MessageId") && s.Data["MessageId"] == messageId)), Times.Once);
        }
    }
}