using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Application.Services.Password
{
    public interface IPasswordService
    {
        Task<SecuredPassword> GenerateAsync(string plainText);
        Task<bool> VerifyAsync(string plainTextPassword, string hashedPassword, string salt, string profileId);
        bool CheckPasswordMatchesRequiredComplexity(string password);
    }
}
