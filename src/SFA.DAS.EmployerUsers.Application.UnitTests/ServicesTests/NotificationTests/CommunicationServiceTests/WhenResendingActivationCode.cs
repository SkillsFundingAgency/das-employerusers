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
        public async Task MyTest()
        {
            var user = new User
            {
                AccessCode = "123456",
                Email = "test@test.com",
                Id = Guid.NewGuid().ToString(),
            };
            var messageId = Guid.NewGuid().ToString();

            var messagePropertiesSerialized = JsonConvert.SerializeObject(CreateMessage(user, messageId));

            //Act
            await _communicationService.ResendActivationCodeMessage(user, messageId);

            //Assert
            _httpClientWrapper.Verify(x => x.SendMessage(It.Is<Dictionary<string, string>>(s => JsonConvert.SerializeObject(s) == messagePropertiesSerialized)), Times.Once);
        }

        private Dictionary<string, string> CreateMessage(User user, string messageId)
        {
            return new Dictionary<string, string>
            {
                {"AccessCode", user.AccessCode},
                {"UserId", user.Id},
                {"MessageId", messageId},
                {"messagetype", "ResendActivationCodeEmail"},
                {"toEmail", user.Email},
                {"fromEmail", "info@sfa.das.gov.uk"}
            };
        }
    }
}