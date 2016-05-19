using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.ApplicationLayer;
using SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Web.Models;

namespace SFA.DAS.EmployerUsers.Web.Orchestrators
{
    public interface IOrchestrator
    {
        
    }

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
    }
}