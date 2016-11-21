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
    public class WhenRegisteringAUser
    {
        private Mock<INotificationsApi> _notificationsApi;
        private CommunicationService _communicationService;

        [SetUp]
        public void Arrange()
        {
            _notificationsApi = new Mock<INotificationsApi>();

            _communicationService = new CommunicationService(_notificationsApi.Object);

        }

        [Test]
        public async Task ThenTheSendUserRegistrationIsCalledWithTheCorrectParameters()
        {
            //Arrange
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


            //Act
            await _communicationService.SendUserRegistrationMessage(user, messageId);

            //Assert
            _notificationsApi.Verify(n => n.SendEmail(It.Is<Email>(e => e.TemplateId == "UserRegistration"
                                                                     && e.RecipientsAddress == "test@test.com"
                                                                     && e.Subject == "Access your apprenticeship levy account"
                                                                     && e.ReplyToAddress == "info@sfa.das.gov.uk"
                                                                     && e.Tokens.ContainsKey("AccessCode") && e.Tokens["AccessCode"] == accessCode)), Times.Once);
        }
    }
}
