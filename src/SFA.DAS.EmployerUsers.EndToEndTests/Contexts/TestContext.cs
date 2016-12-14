using System;

namespace SFA.DAS.EmployerUsers.EndToEndTests.Contexts
{
    public class TestContext
    {
        public TestContext()
        {
            FirstName = SampleData.GetRandomSelectionFrom(SampleData.FirstNames);
            LastName = SampleData.GetRandomSelectionFrom(SampleData.LastNames);
            EmailAddress = $"{Guid.NewGuid()}@endtoendtests.local";
            Password = "dsh1JnK12Ms$$mas";
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}
