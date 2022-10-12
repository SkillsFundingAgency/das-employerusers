using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.UpdateUser;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.UpdateUserCommandTests
{
    public class WhenHandlingUpdateUserCommand
    {
        private UpdateUserCommandHandler _updateUserCommandHandler;
        private Mock<IValidator<UpdateUserCommand>> _updateUserCommandValidator;
        private Mock<IUserRepository> _userRepository;
        private User _expectedUser;
        private const string ExpectedEmail = "test@user.local";
        private const string ExpectedGovIdentifier = "identifier:1231asd123";
        private const string NotAUser = "not@user.local";

        [SetUp]
        public void Arrange()
        {

            _updateUserCommandValidator = new Mock<IValidator<UpdateUserCommand>>();
            _updateUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<UpdateUserCommand>())).ReturnsAsync(new ValidationResult());
            _userRepository = new Mock<IUserRepository>();
            _expectedUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = ExpectedEmail,
            };
            _userRepository.Setup(x => x.GetByEmailAddress(ExpectedEmail))
                          .ReturnsAsync(_expectedUser);
            _userRepository.Setup(x => x.GetByEmailAddress(NotAUser)).ReturnsAsync((User)null);

            _updateUserCommandHandler = new UpdateUserCommandHandler(_updateUserCommandValidator.Object, _userRepository.Object);
        }

        [Test]
        public async Task ThenTheCommandIsCheckedToSeeIfItIsValid()
        {
            //Arrange
            var unlockUserCommand = new UpdateUserCommand { Email = ExpectedEmail };

            //Act
            await _updateUserCommandHandler.Handle(unlockUserCommand);

            //Assert
            _updateUserCommandValidator.Verify(x => x.ValidateAsync(unlockUserCommand), Times.Once);
        }

        [Test]
        public void ThenAnInvalidRequestExceptionIsThrownIfTheCommandIsNotValid()
        {
            //Arrange
            _updateUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<UpdateUserCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _updateUserCommandHandler.Handle(new UpdateUserCommand()));
        }

        [Test]
        public async Task ThenTheUserIsRetrievedFromTheUserRepositoryAndUpdated()
        {
            //Arrange
            var unlockUserCommand = new UpdateUserCommand { Email = ExpectedEmail, GovUkIdentifier = ExpectedGovIdentifier};

            //Act
            var actual = await _updateUserCommandHandler.Handle(unlockUserCommand);

            //Assert
            _userRepository.Verify(x => x.GetByEmailAddress(ExpectedEmail), Times.Once);
            _userRepository.Verify(x => x.UpdateWithGovIdentifier(It.Is<User>(c=>c.Email.Equals(ExpectedEmail)&& c.GovUkIdentifier.Equals(ExpectedGovIdentifier))), Times.Once);
            Assert.AreEqual(_expectedUser.Id, actual.User.Id);
            Assert.AreEqual(_expectedUser.Email, actual.User.Email);
        }

        [Test]
        public async Task ThenTheUserIsNotUpdatedIfNotFoundInRepository()
        {
            //Arrange
            var unlockUserCommand = new UpdateUserCommand { Email = NotAUser, GovUkIdentifier = ExpectedGovIdentifier };

            //Act
            var actual = await _updateUserCommandHandler.Handle(unlockUserCommand);

            //Assert
            _userRepository.Verify(x => x.UpdateWithGovIdentifier(It.IsAny<User>()), Times.Never);
            Assert.IsNull(actual.User);
        }
    }
}