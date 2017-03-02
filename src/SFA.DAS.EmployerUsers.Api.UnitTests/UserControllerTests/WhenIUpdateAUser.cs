using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Controllers;
using SFA.DAS.EmployerUsers.Api.Orchestrators;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.UserControllerTests
{
    public class WhenIUpdateAUser
    {
        private const string UserId = "User001";

        private Mock<UserOrchestrator> _orchestrator;
        private UserController _controller;
        private PatchUserViewModel _patch;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<UserOrchestrator>();
            _orchestrator.Setup(o => o.UpdateUser(It.IsAny<string>(), It.IsAny<PatchUserViewModel>()))
                .ReturnsAsync(new OrchestratorResponse
                {
                    Status = HttpStatusCode.Accepted
                });

            _controller = new UserController(_orchestrator.Object);

            _patch = new PatchUserViewModel { RequiresPasswordReset = true };
        }

        [Test]
        public async Task ThenItShouldReturnAnAcceptedResult()
        {
            // Act
            var actual = await _controller.Update(UserId, _patch);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<AcceptedResult>(actual);
            Assert.AreEqual(HttpStatusCode.Accepted, ((AcceptedResult)actual).StatusCode);
        }

        [Test]
        public async Task ThenItShouldCallTheOrchestrator()
        {
            // Act
            await _controller.Update(UserId, _patch);

            // Assert
            _orchestrator.Verify(o => o.UpdateUser(UserId, It.Is<PatchUserViewModel>(p => p.RequiresPasswordReset == true)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnInternalServerErrorWhenOrchestratorResponseInidcatesFailure()
        {
            // Arrange
            _orchestrator.Setup(o => o.UpdateUser(It.IsAny<string>(), It.IsAny<PatchUserViewModel>()))
                .ReturnsAsync(new OrchestratorResponse
                {
                    Status = HttpStatusCode.InternalServerError,
                    Exception = new System.Exception("Unit test")
                });

            // Act
            var actual = await _controller.Update(UserId, _patch);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<InternalServerErrorResult>(actual);
        }

        [Test]
        public async Task ThenItShouldReturnBadRequestErrorWhenOrchestratorResponseInidcatesInvalidRequest()
        {
            // Arrange
            _orchestrator.Setup(o => o.UpdateUser(It.IsAny<string>(), It.IsAny<PatchUserViewModel>()))
                .ReturnsAsync(new OrchestratorResponse
                {
                    Status = HttpStatusCode.BadRequest,
                    Exception = new InvalidRequestException(new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "", "Some error" }
                    })
                });

            // Act
            var actual = await _controller.Update(UserId, _patch);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actual);
            Assert.AreEqual("Request is invalid:\n: Some error", ((BadRequestErrorMessageResult)actual).Message);
        }
    }
}
