using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.NotificationTests.CommunicationServiceTests
{
    [TestFixture]
    public class WhenResendingActivationCode
    {
        private Mock<INotificationsApi> _notificationsApi;
        private CommunicationService _communicationService;

        [SetUp]
        public void Setup()
        {
            _notificationsApi = new Mock<INotificationsApi>();

            _communicationService = new CommunicationService(_notificationsApi.Object);
        }

        [Test]
        public async Task ThenItShouldSendAHttpMessageWithTheCorrectContent()
        {
            // Arrange
            var accessCode = "123456";
            var returnUrl = "http://abc";
            var expiryTime = DateTime.Now.AddMinutes(10);

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
                        ExpiryTime = expiryTime,
                        ReturnUrl = returnUrl
                    }
                }
            };
            var messageId = Guid.NewGuid().ToString();

            // Act
            await _communicationService.ResendActivationCodeMessage(user, messageId);

            // Assert
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.TemplateId == "ResendActivationCode")), Times.Once);
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.RecipientsAddress == user.Email)), Times.Once);
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.ReplyToAddress == "info@sfa.das.gov.uk")), Times.Once);
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.Subject == "Access your apprenticeship levy account")), Times.Once);
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.Tokens.ContainsKey("AccessCode") && s.Tokens["AccessCode"] == accessCode)), Times.Once);
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.Tokens.ContainsKey("CodeExpiry") && s.Tokens["CodeExpiry"].Equals(expiryTime.ToString("d MMMM yyyy")))), Times.Once);
            _notificationsApi.Verify(x => x.SendEmail(It.Is<Email>(s => s.Tokens.ContainsKey("ReturnUrl") && s.Tokens["ReturnUrl"] == returnUrl)), Times.Once);
            
            
        }
    }
}