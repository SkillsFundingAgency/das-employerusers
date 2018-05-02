using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Application.Queries.GetUnlockCodeLength;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.QueriesTests.GetUnlockCodeLength
{
    public class WhenIGetUnlockCodeLength
    {
        [Test]
        public void ThenIShouldThrowArgumentExceptionIfNoConfig()
        {
            var configurationService = new Mock<IConfigurationService>();
            configurationService.Setup(c => c.GetAsync<EmployerUsersConfiguration>())
                .ReturnsAsync((EmployerUsersConfiguration) null);

            var queryHandler = new GetUnlockCodeDetailsQueryHandler(configurationService.Object);

            Assert.ThrowsAsync<ArgumentException>(() => queryHandler.Handle(new GetUnlockCodeQuery()));
        }

        [Test]
        public void ThenIShouldGetTheCorrectUnlockCodeLentgh()
        {
            const int unlockCodeLength = 99;
            var configurationService = new Mock<IConfigurationService>();
            configurationService.Setup(c => c.GetAsync<EmployerUsersConfiguration>())
                .ReturnsAsync(new EmployerUsersConfiguration
                {
                    Account = new AccountConfiguration
                    {
                        UnlockCodeLength = unlockCodeLength
                    }
                });

            var queryHandler = new GetUnlockCodeDetailsQueryHandler(configurationService.Object);
            var result = queryHandler.Handle(new GetUnlockCodeQuery());

            Assert.AreEqual(unlockCodeLength, result.Result.UnlockCodeLength);
        }
    }
}
