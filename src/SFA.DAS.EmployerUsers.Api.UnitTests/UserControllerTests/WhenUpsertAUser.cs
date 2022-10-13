using System.Threading.Tasks;
using System.Web.Http.Results;
using AutoFixture.NUnit3;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Controllers;
using SFA.DAS.EmployerUsers.Api.Orchestrators;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Commands.UpdateUser;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.UserControllerTests
{
    [TestFixture]
    public class WhenUpsertAUser
    {
        [Test, MoqAutoData]
        public async Task ThenTheEmailAndIdAreUpdatedAndResultReturned(
            string email, 
            string firstName, 
            string lastName,
            string govUkIdentifier,
            UpdateUserCommandResponse response,
            [Frozen] Mock<IMediator> mediator,
            UserOrchestrator userOrchestrator)
        {
            // Arrange
            var controller = new UserController(userOrchestrator);
            mediator.Setup(x =>
                x.SendAsync(It.Is<UpdateUserCommand>(c =>
                    c.Email.Equals(email) && c.GovUkIdentifier.Equals(govUkIdentifier)))).ReturnsAsync(response);

            //Act
            var actual = await controller.Update(email, new UpdateUser(govUkIdentifier, firstName, lastName));

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<UserViewModel>>(actual);
            var model = actual as CreatedNegotiatedContentResult<UserViewModel>;
            Assert.IsNotNull(model);
        }
    }
}