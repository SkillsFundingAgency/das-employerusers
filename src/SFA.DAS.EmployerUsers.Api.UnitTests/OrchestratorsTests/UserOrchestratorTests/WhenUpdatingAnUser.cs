using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Orchestrators;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.UpdateUser;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.OrchestratorsTests.UserOrchestratorTests
{
    public class WhenUpdatingAnUser
    {
        public const string UserId = "User001";

        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private UserOrchestrator _orchestrator;
        private User _user;

        [SetUp]
        public void Arrange()
        {
            _user = new Domain.User
            {
                Id = UserId
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetUserByIdQuery>()))
                .ReturnsAsync(_user);

            _logger = new Mock<ILogger>();

            _orchestrator = new UserOrchestrator(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenItShouldGetTheFullUserObject()
        {
            // Act
            await _orchestrator.UpdateUser(UserId, new Types.PatchUserViewModel { RequiresPasswordReset = true });

            // Assert
            _mediator.Verify(m => m.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == UserId)), Times.Once);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenItShouldUpdateTheUserWithUpdatedFields(bool? requirePasswordReset)
        {
            // Act
            await _orchestrator.UpdateUser(UserId, new Types.PatchUserViewModel { RequiresPasswordReset = requirePasswordReset });

            // Assert
            _mediator.Verify(m => m.SendAsync(It.Is<UpdateUserCommand>(q => q.User.Id == _user.Id && q.User.RequiresPasswordReset == requirePasswordReset)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnAcceptResponse()
        {
            // Act
            var actual = await _orchestrator.UpdateUser(UserId, new Types.PatchUserViewModel { RequiresPasswordReset = true });

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.Accepted, actual.Status);
        }

        [Test]
        public async Task ThenItShouldReturnBadRequestResponseWhenInvalidRequestExceptionOccurs()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<UpdateUserCommand>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "", "Unit test" } }));

            // Act
            var actual = await _orchestrator.UpdateUser(UserId, new Types.PatchUserViewModel { RequiresPasswordReset = true });

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
            Assert.IsNotNull(actual.Exception);
        }

        [Test]
        public async Task ThenItShouldReturnInternalServerResponseWhenUnexpectedExceptionOccurs()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<UpdateUserCommand>()))
                .ThrowsAsync(new Exception("Unit test"));

            // Act
            var actual = await _orchestrator.UpdateUser(UserId, new Types.PatchUserViewModel { RequiresPasswordReset = true });

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.InternalServerError, actual.Status);
            Assert.IsNotNull(actual.Exception);
        }

    }
}
