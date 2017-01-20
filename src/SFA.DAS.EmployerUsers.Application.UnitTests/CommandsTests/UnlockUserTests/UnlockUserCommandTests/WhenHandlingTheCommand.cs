using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.UnlockUser;
using Moq;
using SFA.DAS.EmployerUsers.Application.Events.AccountLocked;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.UnlockUserTests.UnlockUserCommandTests
{
    public class WhenHandlingTheCommand
    {
        private UnlockUserCommandHandler _unlockUserCommand;
        private Mock<IValidator<UnlockUserCommand>> _unlockUserCommandValidator;
        private Mock<IUserRepository> _userRepositry;
        private Mock<IMediator> _mediator;
        private const string AccessCode = "ABC123456PLM";
        private const string ExpectedEmail = "test@user.local";
        private const string NotAUser = "not@user.local";

        [SetUp]
        public void Arrange()
        {

            _unlockUserCommandValidator = new Mock<IValidator<UnlockUserCommand>>();
            _unlockUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<UnlockUserCommand>())).ReturnsAsync(new ValidationResult());
            _userRepositry = new Mock<IUserRepository>();
            _mediator = new Mock<IMediator>();
            _userRepositry.Setup(x => x.GetByEmailAddress(ExpectedEmail))
                          .ReturnsAsync(new User
                          {
                              Email = ExpectedEmail,
                              IsLocked = true,
                              SecurityCodes = new[]
                              {
                                  new SecurityCode
                                  {
                                      Code = AccessCode,
                                      CodeType = SecurityCodeType.UnlockCode,
                                      ExpiryTime = DateTime.MaxValue
                                  },
                                  new SecurityCode
                                  {
                                      Code = AccessCode + "A",
                                      CodeType = SecurityCodeType.AccessCode,
                                      ExpiryTime = DateTime.MaxValue
                                  },
                                  new SecurityCode
                                  {
                                      Code = AccessCode + "B",
                                      CodeType = SecurityCodeType.PasswordResetCode,
                                      ExpiryTime = DateTime.MaxValue
                                  }
                              }
                          });
            _userRepositry.Setup(x => x.GetByEmailAddress(NotAUser)).ReturnsAsync((User)null);
            _unlockUserCommand = new UnlockUserCommandHandler(_unlockUserCommandValidator.Object, _userRepositry.Object, _mediator.Object);
        }

        [Test]
        public async Task ThenTheCommandIsCheckedToSeeIfItIsValid()
        {
            //Arrange
            var unlockUserCommand = new UnlockUserCommand { Email = ExpectedEmail };

            //Act
            await _unlockUserCommand.Handle(unlockUserCommand);

            //Assert
            _unlockUserCommandValidator.Verify(x => x.ValidateAsync(unlockUserCommand), Times.Once);
        }

        [Test]
        public void ThenAnInvalidRequestExceptionIsThrownIfTheCommandIsNotValid()
        {
            //Arrange
            _unlockUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<UnlockUserCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _unlockUserCommand.Handle(new UnlockUserCommand()));
        }

        [Test]
        public async Task ThenTheUserRespositoryIsCalledIfTheCommandIsValid()
        {
            //Arrange
            var unlockUserCommand = new UnlockUserCommand { Email = ExpectedEmail };

            //Act
            await _unlockUserCommand.Handle(unlockUserCommand);

            //Assert
            _userRepositry.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task ThenTheUserIsRetrievedFromTheUserRepository()
        {
            //Arrange
            var unlockUserCommand = new UnlockUserCommand { Email = ExpectedEmail };

            //Act
            await _unlockUserCommand.Handle(unlockUserCommand);

            //Assert
            _userRepositry.Verify(x => x.GetByEmailAddress(ExpectedEmail), Times.Once);
        }

        [Test]
        public async Task ThenTheUserIsUnlockedAndUnlockCodesExpired()
        {
            //Arrange
            var unlockUserCommand = new UnlockUserCommand
            {
                UnlockCode = AccessCode,
                Email = ExpectedEmail
            };

            //Act
            await _unlockUserCommand.Handle(unlockUserCommand);

            //Assert
            _userRepositry.Verify(x => x.Update(It.Is<User>(c => !c.IsActive 
                                                              && c.Email == ExpectedEmail 
                                                              && !c.IsLocked
                                                              && c.FailedLoginAttempts == 0
                                                              && !c.SecurityCodes.Any(sc => sc.CodeType == SecurityCodeType.UnlockCode))), 
                                  Times.Once);
        }

        [Test]
        public void ThenTheUserRepositoryIsNotUpdatedIfTheUserDoesNotExist()
        {
            //Arrange
            _unlockUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<UnlockUserCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = { { "", "" } } });
            var unlockUserCommand = new UnlockUserCommand
            {
                UnlockCode = AccessCode,
                Email = NotAUser
            };

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _unlockUserCommand.Handle(unlockUserCommand));

            //Assert
            _userRepositry.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void ThenAnAccountLockedEventIsRaisedIfTheValidationFailsAndTheUserIsNotNull()
        {
            //Arrange
            _unlockUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<UnlockUserCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = { { "", "" } } });
            var unlockUserCommand = new UnlockUserCommand
            {
                UnlockCode = AccessCode,
                Email = ExpectedEmail,
                User = new User
                {
                    Email = ExpectedEmail
                }
            };

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _unlockUserCommand.Handle(unlockUserCommand));

            //Assert
            _mediator.Verify(x => x.PublishAsync(It.Is<AccountLockedEvent>(c => c.User.Email.Equals(ExpectedEmail))), Times.Once);
        }


        [Test]
        public void ThenAnAccountLockedEventIsNotRaisedIfTheValidationFailsAndTheUserIsNull()
        {
            //Arrange
            _unlockUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<UnlockUserCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = { { "", "" } } });
            var unlockUserCommand = new UnlockUserCommand
            {
                UnlockCode = AccessCode,
                Email = NotAUser
            };

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _unlockUserCommand.Handle(unlockUserCommand));

            //Assert
            _mediator.Verify(x => x.PublishAsync(It.IsAny<AccountLockedEvent>()), Times.Never());
        }

        [Test]
        public void ThenAnArgumentNullExceptionIsThrownIfTheCommandIsNull()
        {
            //Act
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _unlockUserCommand.Handle(null));
        }
        
        [Test]
        public async Task ThenTheRespostioryWontBeUpdatedIfTheAccountIsNotLocked()
        {
            //Arrange
            _userRepositry.Setup(x => x.GetByEmailAddress(ExpectedEmail))
                          .ReturnsAsync(new User
                          {
                              Email = ExpectedEmail,
                              IsLocked = false,
                              SecurityCodes = new[]
                              {
                                  new SecurityCode
                                  {
                                      Code = AccessCode,
                                      CodeType = SecurityCodeType.AccessCode,
                                      ExpiryTime = DateTime.MaxValue
                                  }
                              }
                          });
            var unlockUserCommand = new UnlockUserCommand
            {
                UnlockCode = "ASDASDASD",
                Email = ExpectedEmail
            };

            //Act
            await _unlockUserCommand.Handle(unlockUserCommand);

            //Assert
            _userRepositry.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

    }
}
