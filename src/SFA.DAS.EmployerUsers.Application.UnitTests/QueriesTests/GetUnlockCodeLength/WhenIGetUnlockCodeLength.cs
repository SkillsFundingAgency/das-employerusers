using System;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.GetUnlockCodeLength;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.QueriesTests.GetUnlockCodeLength
{
    public class WhenIGetUnlockCodeLength
    {
        [Test]
        public void ThenIShouldThrowArgumentExceptionIfNoConfig()
        {
            EmployerUsersConfiguration configuration = null;

            var queryHandler = new GetUnlockCodeDetailsQueryHandler(configuration);

            Assert.ThrowsAsync<ArgumentException>(() => queryHandler.Handle(new GetUnlockCodeQuery()));
        }

        [Test]
        public void ThenIShouldGetTheCorrectUnlockCodeLentgh()
        {
            const int unlockCodeLength = 99;
            var configuration = new EmployerUsersConfiguration
            {
                Account = new AccountConfiguration
                {
                    UnlockCodeLength = unlockCodeLength
                }
            };

            var queryHandler = new GetUnlockCodeDetailsQueryHandler(configuration);
            var result = queryHandler.Handle(new GetUnlockCodeQuery());

            Assert.AreEqual(unlockCodeLength, result.Result.UnlockCodeLength);
        }
    }
}
