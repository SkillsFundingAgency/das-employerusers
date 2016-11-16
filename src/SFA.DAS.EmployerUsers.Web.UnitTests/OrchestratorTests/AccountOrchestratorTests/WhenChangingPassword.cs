using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.ChangePassword;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenChangingPassword
    {
        private const string UserId = "User1";
        private const string CurrentPassword = "Password";
        private const string NewPassword = "NewPassword";

        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountOrchestrator _orchestrator;
        private ChangePasswordViewModel _model;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == UserId)))
                .Returns(Task.FromResult(new Domain.User
                {
                    Id = UserId
                }));

            _owinWrapper = new Mock<IOwinWrapper>();

            _orchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object);

            _model = new ChangePasswordViewModel
            {
                UserId = UserId,
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmPassword = NewPassword
            };
        }

        [Test]
        public async Task ThenItShouldReturnAValidModelIfChangeSuccessful()
        {
            // Act
            var actual = await _orchestrator.ChangePassword(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Valid);
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidModelWithErrorsIfAValidationErrorOccurs()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.Is<ChangePasswordCommand>(c => c.User.Id == UserId
                                                                            && c.CurrentPassword == CurrentPassword
                                                                            && c.NewPassword == NewPassword
                                                                            && c.ConfirmPassword == NewPassword)))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "", "Not valid" } }));
            // Act
            var actual = await _orchestrator.ChangePassword(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Valid);
            Assert.IsNotNull(actual.ErrorDictionary);
            Assert.IsTrue(actual.ErrorDictionary.ContainsKey(""));
            Assert.AreEqual("Not valid", actual.ErrorDictionary[""]);
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidModelWithAnErrorIfRequestErrors()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<ChangePasswordCommand>()))
                .ThrowsAsync(new Exception("A problem has occured"));

            // Act
            var actual = await _orchestrator.ChangePassword(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Valid);
            Assert.IsNotNull(actual.ErrorDictionary);
            Assert.IsTrue(actual.ErrorDictionary.ContainsKey(""));
            Assert.AreEqual("A problem has occured", actual.ErrorDictionary[""]);
        }
    }
}
