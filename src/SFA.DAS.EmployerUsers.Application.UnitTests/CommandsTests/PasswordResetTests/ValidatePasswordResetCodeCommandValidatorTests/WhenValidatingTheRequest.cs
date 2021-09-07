using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.PasswordReset;
using SFA.DAS.EmployerUsers.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.PasswordResetTests.ValidatePasswordResetCodeCommandValidatorTests
{
    public class WhenValidatingTheRequest
    {
        private ValidatePasswordResetCodeCommandValidator _sut;

        [SetUp]
        public void Arrange()
        {
            ConfigurationManager.AppSettings["UseStaticCodeGenerator"] = "false";

            _sut = new ValidatePasswordResetCodeCommandValidator();
        }

        [Test]
        public async Task ThenFalseIsReturnedWhenTheMessageHasNoUser()
        {
            // Act
            var actual = await _sut.ValidateAsync(new ValidatePasswordResetCodeCommand());

            // Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedIfThePasscodeDoesNotMatch()
        {
            // Act
            var actual = await _sut.ValidateAsync(new ValidatePasswordResetCodeCommand
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

            // Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("PasswordResetCode", "Reset code is invalid"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenFalseIsReturnedIfThePasscodeMatchesButHasExpired()
        {
            // Act
            var actual = await _sut.ValidateAsync(new ValidatePasswordResetCodeCommand
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

            // Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("PasswordResetCode", "Reset code has expired"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenTrueIsReturnedIfPasswordResetCodeMatchesHasNotExpiredAndDoesNotHaveTooManyFailedAttempts()
        {
            // Act
            var actual = await _sut.ValidateAsync(new ValidatePasswordResetCodeCommand
            {
                PasswordResetCode = "123456ABC",
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "123456ABC",
                            CodeType = SecurityCodeType.PasswordResetCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                }
            });

            // Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
