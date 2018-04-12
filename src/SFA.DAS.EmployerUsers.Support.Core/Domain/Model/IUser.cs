namespace SFA.DAS.EmployerUsers.Support.Core.Domain.Model
{
    public interface IUser
    {
        string Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        bool IsActive { get; set; }
        bool IsLocked { get; set; }

        UserStatus Status { get; }
    }
}