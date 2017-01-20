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
    public class WhenSendingANotification
    {
        private Mock<INotificationsApi> _notificationsApi;
        private Mock<ILogger> _logger;
        private CommunicationService _communicationService;

        [SetUp]
        public void Arrange()
        {
            _notificationsApi = new Mock<INotificationsApi>();
            _logger = new Mock<ILogger>();

            _communicationService = new CommunicationService(_notificationsApi.Object, _logger.Object);
        }

        [Test]
        public async Task ThenAMessageIsLoggedWhenAnExceptionIsThrown()
        {
            //Arrange
            var user = new User
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "Tester",
                SecurityCodes = new []
                {
                    new SecurityCode {Code="123",CodeType=SecurityCodeType.AccessCode},
                    new SecurityCode {Code="123",CodeType=SecurityCodeType.ConfirmEmailCode},
                    new SecurityCode {Code="123",CodeType=SecurityCodeType.PasswordResetCode},
                    new SecurityCode {Code="123",CodeType=SecurityCodeType.UnlockCode},
                    
                }
            };
            _notificationsApi.Setup(x => x.SendEmail(It.IsAny<Email>())).Throws(new Exception("Exception"));

            //Act
            await _communicationService.SendUserRegistrationMessage(user, "123");
            await _communicationService.SendAccountLockedMessage(user, "123");
            await _communicationService.ResendActivationCodeMessage(user, "123");
            await _communicationService.SendPasswordResetCodeMessage(user, "123");
            await _communicationService.SendConfirmEmailChangeMessage(user, "123");

            //Assert
            _logger.Verify(x=>x.Error(It.IsAny<Exception>(),It.IsAny<string>()),Times.Exactly(5));
        }
    }
}
