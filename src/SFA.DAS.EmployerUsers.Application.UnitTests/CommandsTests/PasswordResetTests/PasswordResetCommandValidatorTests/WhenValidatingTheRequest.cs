using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.PasswordReset;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.PasswordResetTests.PasswordResetCommandValidatorTests
{
    public class WhenValidatingTheRequest
    {
        private PasswordResetCommandValidator _validator;
        private Mock<IPasswordService> _passwordService;

        [SetUp]
        public void Arrange()
        {
            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(x => x.CheckPasswordMatchesRequiredComplexity(It.IsAny<string>())).Returns(true);

            _validator = new PasswordResetCommandValidator(_passwordService.Object);
        }

        [Test]
        public async Task ThenFalseIsReturnedWhenTheMessageHasNoUser()
        {
            //Act
            var actual = await _validator.ValidateAsync(new PasswordResetCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedIfThePasscodeDoesNotMatch()
        {
            //Act
            var actual = await _validator.ValidateAsync(new PasswordResetCommand
            {
                PasswordResetCode = "123456",
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "654321",
                            CodeType = SecurityCodeType.PasswordResetCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                }
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("PasswordResetCode", "Reset code is invalid, try again"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenFalseIsReturnedIfThePasscodeMatchesButHasExpired()
        {
            //Act
            var actual = await _validator.ValidateAsync(new PasswordResetCommand
            {
                PasswordResetCode = "123456",
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "123456",
                            CodeType = SecurityCodeType.PasswordResetCode,
                            ExpiryTime = DateTime.MinValue
                        }
                    }
                }
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("PasswordResetCode", "Reset code has expired, try again"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenFalseIsReturnedfIfThePasswordsDoNotMatch()
        {
            //Act
            var actual = await _validator.ValidateAsync(new PasswordResetCommand
            {
                PasswordResetCode = "654321",
                Password = "654321abc",
                ConfirmPassword = "654321aBc",
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "654321",
                            CodeType = SecurityCodeType.PasswordResetCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                }
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("ConfirmPassword", "Sorry, your passwords don’t match"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenTrueIsReturnedIfAllFieldsHaveBeenSupplied()
        {
            //Act
            var actual = await _validator.ValidateAsync(new PasswordResetCommand
            {
                PasswordResetCode = "123456ABC",
                Password = "abc123YHN",
                ConfirmPassword = "abc123YHN",
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "123456abc",
                            CodeType = SecurityCodeType.PasswordResetCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                }
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenTheErrorDictionaryIsPopulatedIfThePasswordIsNotComplexEnough()
        {
            //arrange 
            _passwordService.Setup(x => x.CheckPasswordMatchesRequiredComplexity(It.IsAny<string>())).Returns(false);

            //Act
            var actual = await _validator.ValidateAsync(new PasswordResetCommand
            {
                PasswordResetCode = "654321",
                Password = "123456",
                ConfirmPassword = "123456",
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "654321",
                            CodeType = SecurityCodeType.PasswordResetCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                }
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("Password", "Password requires upper and lowercase letters, a number and at least 8 characters"), actual.ValidationDictionary);
        }

    }
}
