using System;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.TestHelpers
{
    public class FakeTimeProvider : DateTimeProvider
    {
        public FakeTimeProvider(DateTime currentDateTime)
        {
            UtcNow = currentDateTime;
        }

        public override DateTime UtcNow { get; }
    }
}