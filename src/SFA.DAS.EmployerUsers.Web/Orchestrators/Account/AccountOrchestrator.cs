using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Web.Models;

namespace SFA.DAS.EmployerUsers.Web.Orchestrators.Account
{
    public class AccountOrchestrator
    {
        private readonly IMediator _mediator;
        
        public AccountOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ConfirmationViewModel> Register(RegisterViewModel registerUserViewModel)
        {
            
            await _mediator.SendAsync(new RegisterUserCommand
            {
                FirstName = registerUserViewModel.FirstName,
                LastName = registerUserViewModel.LastName,
                Email = registerUserViewModel.Email,
                ConfirmEmail = registerUserViewModel.ConfirmEmail,
                Password = registerUserViewModel.Password,
                ConfirmPassword = registerUserViewModel.ConfirmPassword
            });
            

            return new ConfirmationViewModel();
        }
    }
}