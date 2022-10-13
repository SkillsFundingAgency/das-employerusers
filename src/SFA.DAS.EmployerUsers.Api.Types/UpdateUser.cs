namespace SFA.DAS.EmployerUsers.Api.Types
{
    public class UpdateUser
    {
        public string GovUkIdentifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UpdateUser(string govUkIdentifier, string firstName, string lastName)
        {
            GovUkIdentifier = govUkIdentifier;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}