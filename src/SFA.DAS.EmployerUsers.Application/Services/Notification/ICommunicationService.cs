﻿using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public interface ICommunicationService
    {
        Task SendUserRegistrationMessage(User user, string messageId);

        Task SendUserAccountConfirmationMessage(User user, string messageId);

        Task SendAccountLockedMessage(User user, string messageId);

        Task ResendActivationCodeMessage(User user, string messageId);
        Task ResendLastActivationCodeMessage(User user, string messageId);
        Task SendUserUnlockedMessage(User user, string messageId);

        Task SendPasswordResetCodeMessage(User user, string messageId, string getForgottenPasswordUrl);

        Task SendConfirmEmailChangeMessage(User user, string messageId);
        Task SendNoAccountToPasswordResetMessage(string emailAddress, string messageId, string registerUrl);
       
    }
}
