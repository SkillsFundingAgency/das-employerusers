namespace SFA.DAS.EmployerUsers.Domain
{
    public enum SearchCategory
    {
        None,
        User,
        Account,
        Apprentice
    }
    
    public class UserSearchModel
    {
        public string Id { get; set; }
        public string Name => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailSearchKeyWord { get => Email?.ToLower(); }
        public string FirstNameSearchKeyWord { get => FirstName?.ToLower(); }
        public string LastNameSearchKeyWord { get => LastName?.ToLower(); }
        public string NameSearchKeyWord { get => Name?.ToLower(); }
        public bool IsSuspended { get; set; }
        public SearchCategory SearchType { get; set; }
    }
}