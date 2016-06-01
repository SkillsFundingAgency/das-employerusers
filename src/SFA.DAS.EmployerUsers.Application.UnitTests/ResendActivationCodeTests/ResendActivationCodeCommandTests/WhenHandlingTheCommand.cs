using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ResendActivationCodeTests.ResendActivationCodeCommandTests
{
    [TestFixture]
    public class WhenHandlingTheCommand
    {
        private Mock<IValidator<ResendActivationCodeCommand>> _validator;
        private Mock<IUserRepository> _userRepository;
        private Mock<ICommunicationService> _communicationSerivce;
        private ResendActivationCodeCommandHandler _commandHandler;

        [SetUp]
        public void Setup()
        {
            _validator = new Mock<IValidator<ResendActivationCodeCommand>>();
            _userRepository = new Mock<IUserRepository>();
            _communicationSerivce = new Mock<ICommunicationService>();
            _commandHandler = new ResendActivationCodeCommandHandler(_validator.Object, _userRepository.Object, _communicationSerivce.Object);
        }

        [Test]
        public void ThenThrowsExceptionIfCommandFailsValidation()
        {
            SetValidatorToReturn(false);

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _commandHandler.Handle(new ResendActivationCodeCommand()));
        }

        [Test]
        public void ThenThrowsExceptionIfUserNotFound()
        {
            SetValidatorToReturn(true);
            SetUserRepositoryToReturn(null);

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _commandHandler.Handle(new ResendActivationCodeCommand()));
        }

        [Test]
        public async Task ThenShouldCallResendIfNotActiveUserFound()
        {
            SetValidatorToReturn(true);
            SetUserRepositoryToReturn(new User
            {
                IsActive = false
            });

            await _commandHandler.Handle(new ResendActivationCodeCommand());

            _communicationSerivce.Verify(x => x.ResendActivationCodeMessage(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ThenShouldNotCallResendIfActiveUserFound()
        {
            SetValidatorToReturn(true);
            SetUserRepositoryToReturn(new User
            {
                IsActive = true
            });

            await _commandHandler.Handle(new ResendActivationCodeCommand());

            _communicationSerivce.Verify(x => x.ResendActivationCodeMessage(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        private void SetValidatorToReturn(bool isValid)
        {
            _validator.Setup(x => x.Validate(It.IsAny<ResendActivationCodeCommand>())).Returns(isValid);
        }

        private void SetUserRepositoryToReturn(User user)
        {
            _userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
        }
    }
}