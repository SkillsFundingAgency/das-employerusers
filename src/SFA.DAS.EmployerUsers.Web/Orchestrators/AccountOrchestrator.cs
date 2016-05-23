using System;
using System.Threading.Tasks;
using IdentityServer3.Core.Extensions;
using MediatR;
using Microsoft.Owin;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;

namespace SFA.DAS.EmployerUsers.Web.Orchestrators
{
    public class AccountOrchestrator : IOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IOwinWrapper _owinWrapper;

        public AccountOrchestrator()
        {
            
        }

        public AccountOrchestrator(IMediator mediator, IOwinWrapper owinWrapper)
        {
            _mediator = mediator;
            _owinWrapper = owinWrapper;
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

                _owinWrapper.IssueLoginCookie(user.Email, $"{user.FirstName} {user.LastName}");
                _owinWrapper.RemovePartialLoginCookie();

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


                SignInUser(registerUserViewModel);

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

        public virtual void SignInUser(RegisterViewModel registerUserViewModel)
        {
            _owinWrapper.IssueLoginCookie(registerUserViewModel.Email, $"{registerUserViewModel.FirstName} {registerUserViewModel.LastName}");
        }
    }
}