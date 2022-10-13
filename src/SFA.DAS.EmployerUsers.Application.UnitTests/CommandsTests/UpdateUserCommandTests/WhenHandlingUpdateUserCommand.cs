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
        private const string ExpectedFirstName = "test";
        private const string ExpectedLastName = "tester";
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
            var updateUserCommand = new UpdateUserCommand
            {
                Email = ExpectedEmail, 
                GovUkIdentifier = ExpectedGovIdentifier,
                FirstName = ExpectedFirstName,
                LastName = ExpectedLastName
            };

            //Act
            var actual = await _updateUserCommandHandler.Handle(updateUserCommand);

            //Assert
            _userRepository.Verify(x => x.GetByEmailAddress(ExpectedEmail), Times.Once);
            _userRepository.Verify(x => x.UpsertWithGovIdentifier(
                It.Is<User>(c=>
                    c.Email.Equals(ExpectedEmail) 
                    && c.GovUkIdentifier.Equals(ExpectedGovIdentifier)
                    && c.FirstName.Equals(ExpectedFirstName)
                    && c.LastName.Equals(ExpectedLastName)
                )), Times.Once);
            Assert.AreEqual(_expectedUser.Id, actual.User.Id);
            Assert.AreEqual(_expectedUser.Email, actual.User.Email);
        }

        
        [Test]
        public async Task TheTheUserIsReturnedByEmail()
        {
            //Arrange
            _expectedUser.GovUkIdentifier = ExpectedGovIdentifier;
            var updateUserCommand = new UpdateUserCommand { Email = ExpectedEmail, GovUkIdentifier = ExpectedGovIdentifier };

            //Act
            var actual = await _updateUserCommandHandler.Handle(updateUserCommand);

            //Assert
            _userRepository.Verify(x => x.UpsertWithGovIdentifier(It.Is<User>(c=>c.GovUkIdentifier.Equals(ExpectedGovIdentifier))), Times.Once);
            Assert.AreEqual(_expectedUser.Id, actual.User.Id);
            Assert.AreEqual(_expectedUser.Email, actual.User.Email);
        }
    }
}