using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ChangePassword;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ChangePasswordTests.ChangePasswordCommandHandlerTests
{
    public class WhenHandling
    {
        private const string UserId = "USER1";
        private const string CurrentPassword = "Password";
        private const string NewPassword = "Password1";
        private const string NewHash = "NEWHASH";
        private const string NewProfileId = "NEWPROFILE";
        private const string NewSalt = "NEWSALT";

        private Mock<IValidator<ChangePasswordCommand>> _validator;
        private Mock<IPasswordService> _passwordService;
        private Mock<IUserRepository> _userRepository;
        private ChangePasswordCommandHandler _handler;
        private ChangePasswordCommand _command;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<ChangePasswordCommand>>();
            _validator.Setup(v => v.ValidateAsync(It.Is<ChangePasswordCommand>(c => c.User.Id == UserId
                                                                            && c.CurrentPassword == CurrentPassword
                                                                            && c.NewPassword == NewPassword
                                                                            && c.ConfirmPassword == NewPassword)))
                .ReturnsAsync(new ValidationResult());

            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(s => s.GenerateAsync(NewPassword))
                .Returns(Task.FromResult(new SecuredPassword
                {
                    HashedPassword = NewHash,
                    ProfileId = NewProfileId,
                    Salt = NewSalt
                }));

            _userRepository = new Mock<IUserRepository>();

            _handler = new ChangePasswordCommandHandler(_validator.Object, _passwordService.Object, _userRepository.Object);

            _command = new ChangePasswordCommand
            {
                User = new User
                {
                    Id = UserId,
                    Password = "OLDHASH",
                    PasswordProfileId = "OLDPROFILE",
                    Salt = "OLDSALT"
                },
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmPassword = NewPassword
            };
        }

        [Test]
        public void ThenItShouldThrowAnInvalidRequestExceptionIfCommandInvalid()
        {
            // Arrange
            _validator.Setup(v => v.ValidateAsync(It.IsAny<ChangePasswordCommand>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "Error" } } });

            // Act + Assert
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));
            Assert.IsNotNull(actual.ErrorMessages);
            Assert.IsTrue(actual.ErrorMessages.ContainsKey(""));
            Assert.AreEqual("Error", actual.ErrorMessages[""]);
        }

        [Test]
        public async Task ThenItShouldUpdateTheUserWithHashedVersionOfNewPassword()
        {
            // Act
            await _handler.Handle(_command);

            // Arrange
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.Id == UserId
                                                               && u.Password == NewHash
                                                               && u.PasswordProfileId == NewProfileId
                                                               && u.Salt == NewSalt)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldNotUpdateUserIfCommandInvalid()
        {
            // Arrange
            _validator.Setup(v => v.ValidateAsync(It.IsAny<ChangePasswordCommand>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "Error" } } });

            // Act
            try
            {
                await _handler.Handle(_command);
            }
            catch (InvalidRequestException)
            {
            }

            // Arrange
            _userRepository.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        }
    }
}
