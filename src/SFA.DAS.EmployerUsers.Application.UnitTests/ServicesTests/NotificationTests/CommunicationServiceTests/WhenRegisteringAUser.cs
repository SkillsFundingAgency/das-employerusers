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
    public class WhenRegisteringAUser
    {
        private CommunicationService _communicationService;
        private Mock<IHttpClientWrapper> _httpClientWrapper;

        [SetUp]
        public void Arrange()
        {
            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _communicationService = new CommunicationService(_httpClientWrapper.Object);
            
        }

        [Test]
        public async Task ThenTheSendUserRegistrationIsCalledWithTheCorrectParameters()
        {
            //Arrange
            var user = new User
            {
                AccessCode = "123456",
                Email = "test@test.com",
                Id = Guid.NewGuid().ToString(),
            };
            var messageId = Guid.NewGuid().ToString();
            var messageProperties = new Dictionary<string, string>
            {
                {"AccessCode", user.AccessCode},
                {"UserId", user.Id},
                {"MessageId", messageId},
                {"messagetype", "SendEmail"},
                {"toEmail", user.Email},
                {"fromEmail", "info@sfa.das.gov.uk"}
            };

            var messagePropertiesSerialized = JsonConvert.SerializeObject(messageProperties);

            //Act
            await _communicationService.SendUserRegistrationMessage(user, messageId);

            //Assert
            _httpClientWrapper.Verify(x=>x.SendMessage(It.Is<Dictionary<string,string>>(s=> JsonConvert.SerializeObject(s) == messagePropertiesSerialized)),Times.Once);
        }
    }
}
