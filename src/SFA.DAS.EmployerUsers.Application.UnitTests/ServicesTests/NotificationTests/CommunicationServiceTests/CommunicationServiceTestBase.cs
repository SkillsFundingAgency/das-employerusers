using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public abstract class CommunicationServiceTestBase
    {
        protected const string MessageId = "MESSAGE";
        protected const string Email = "user.one@unit.tests";

        protected Mock<INotificationsApi> NotificationsApi;
        protected Mock<ILogger> Logger;
        protected CommunicationService CommunicationService;
        protected User User;

        protected virtual void Arrange()
        {
            NotificationsApi = new Mock<INotificationsApi>();
            Logger = new Mock<ILogger>();
            CommunicationService = new CommunicationService(NotificationsApi.Object, Logger.Object);

            User = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = Email
            };

            ExpectedRecipientsAddress = User.Email;
            ExpectedReplyToAddress = "info@sfa.das.gov.uk";
        }

        protected string ExpectedTemplateId { get; set; }
        protected string ExpectedSubject { get; set; }
        protected string ExpectedRecipientsAddress { get; set; }
        protected string ExpectedReplyToAddress { get; set; }

        protected abstract Task Act();

        [Test]
        public async Task ThenItShouldUseMessageIdAsSystemId()
        {
            // Act
            await Act();

            // Assert
            NotificationsApi.Verify(c => c.SendEmail(It.Is<Email>(n => n.SystemId == MessageId)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldUseCorrectTemplateId()
        {
            // Act
            await Act();

            // Assert
            NotificationsApi.Verify(c => c.SendEmail(It.Is<Email>(n => n.TemplateId == ExpectedTemplateId)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldUseCorrectSubject()
        {
            // Act
            await Act();

            // Assert
            NotificationsApi.Verify(c => c.SendEmail(It.Is<Email>(n => n.Subject == ExpectedSubject)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldUseCorrectRecipientsAddress()
        {
            // Act
            await Act();

            // Assert
            NotificationsApi.Verify(c => c.SendEmail(It.Is<Email>(n => n.RecipientsAddress == ExpectedRecipientsAddress)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldUseCorrectReplyToAddress()
        {
            // Act
            await Act();

            // Assert
            NotificationsApi.Verify(c => c.SendEmail(It.Is<Email>(n => n.ReplyToAddress == ExpectedReplyToAddress)), Times.Once);
        }



        protected async Task ThenItShouldIncludeCorrectTokenValue(string key, string value)
        {
            // Act
            await Act();

            // Assert
            NotificationsApi.Verify(c => c.SendEmail(It.Is<Email>(n => n.Tokens.ContainsKey(key) && n.Tokens[key] == value)), Times.Once);
        }
    }
}
