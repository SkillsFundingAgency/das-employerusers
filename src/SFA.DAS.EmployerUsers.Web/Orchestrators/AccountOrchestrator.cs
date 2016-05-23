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

        public AccountOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public virtual async Task<bool> Login(LoginViewModel loginViewModel, IOwinContext owinContext)
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


                var env = owinContext.Environment;
                env.IssueLoginCookie(new IdentityServer3.Core.Models.AuthenticatedLogin
                {
                    Subject = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                });
                env.RemovePartialLoginCookie();

                return true;
            }
            catch (Exception)
            {
                //TODO: Log?
                return false;
            }
        }

        public virtual async Task<bool> Register(RegisterViewModel registerUserViewModel, IOwinContext owinContext)
        {
            try
            {
                var userId = Guid.NewGuid().ToString();
                await _mediator.SendAsync(new RegisterUserCommand
                {
                    Id = userId,
                    FirstName = registerUserViewModel.FirstName,
                    LastName = registerUserViewModel.LastName,
                    Email = registerUserViewModel.Email,
                    Password = registerUserViewModel.Password,
                    ConfirmPassword = registerUserViewModel.ConfirmPassword
                });

                var env = owinContext.Environment;
                env.IssueLoginCookie(new IdentityServer3.Core.Models.AuthenticatedLogin
                {
                    Subject = userId,
                    Name = $"{registerUserViewModel.FirstName} {registerUserViewModel.LastName}",
                });
                env.RemovePartialLoginCookie();

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