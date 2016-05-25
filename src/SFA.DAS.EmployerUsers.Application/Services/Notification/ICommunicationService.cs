using SFA.DAS.EmployerUsers.Application.Services.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public interface ICommunicationService : IDisposable
    {
        Task SendUserRegistrationMessage(User user);

        Task SendMessage(Dictionary<string, string> messageProperties);
    }
}
