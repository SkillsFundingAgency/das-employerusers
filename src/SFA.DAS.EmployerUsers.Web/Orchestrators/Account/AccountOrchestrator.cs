using System;
using System.Collections.Generic;
using System.Linq;
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

        public ConfirmationViewModel Register(RegisterViewModel registerUserViewModel)
        {

            _mediator.Send(new RegisterUserCommand());
            
            return new ConfirmationViewModel();
        }
    }
}