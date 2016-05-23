using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Web.Models;

namespace SFA.DAS.EmployerUsers.Web.Orchestrators
{
    public class AccountOrchestrator : IOrchestrator
    {
        private readonly IMediator _mediator;

        public AccountOrchestrator()
        {
            
        }

        public AccountOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public virtual async Task<bool> Login(LoginViewModel loginViewModel)
        {
            try
            {
                var user = await _mediator.SendAsync(new AuthenticateUserCommand
                {
                    EmailAddress = loginViewModel.EmailAddress,
                    Password = loginViewModel.Password
                });
                if (user == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                //TODO: Log?
                return false;
            }
        }

        public virtual async Task<bool> Register(RegisterViewModel registerUserViewModel)
        {
            try
            {
                await _mediator.SendAsync(new RegisterUserCommand
                {
                    FirstName = registerUserViewModel.FirstName,
                    LastName = registerUserViewModel.LastName,
                    Email = registerUserViewModel.Email,
                    Password = registerUserViewModel.Password,
                    ConfirmPassword = registerUserViewModel.ConfirmPassword
                });


                return true;
            }
            catch (InvalidRequestException)
            {
                return false;
            }
            
        }

        public virtual async Task<bool> ActivateUser(AccessCodeViewModel accessCodeviewModel)
        {
            try
            {
                await _mediator.SendAsync(new ActivateUserCommand
                {
                    AccessCode = accessCodeviewModel.AccessCode,
                    UserId = accessCodeviewModel.UserId
                });

                return true;
            }
            catch (InvalidRequestException)
            {
                return false;
            }
            
        }
    }
}