using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public interface ICommunicationService
    {
        Task SendUserRegistrationMessage(User user, string messageId);

        Task SendUserAccountConfirmationMessage(User user, string messageId);
        Task SendAccountLockedMessage(User user, string messageId);
    }
}
