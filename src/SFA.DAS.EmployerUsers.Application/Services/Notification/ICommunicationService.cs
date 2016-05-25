using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public interface ICommunicationService
    {
        Task SendUserRegistrationMessage(User user, string messageId);
        
    }
}
