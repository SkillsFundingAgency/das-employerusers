using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public interface IHttpClientWrapper 
    {
        Task SendMessage<T>(T content);
    }
}