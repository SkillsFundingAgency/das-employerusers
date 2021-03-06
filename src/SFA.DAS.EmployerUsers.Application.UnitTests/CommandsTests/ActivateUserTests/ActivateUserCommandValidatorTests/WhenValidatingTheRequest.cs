﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ActivateUserTests.ActivateUserCommandValidatorTests
{
    [ExcludeFromCodeCoverage]
    public class WhenValidatingTheRequest
    {
        private ActivateUserCommandValidator _activateUserCommandValidator;
        private const string InvalidCodeMessage = "Code is not recognised";
        private const string MissingCodeMessage = "Enter the code we emailed you";


        [SetUp]
        public void Arrange()
        {
            _activateUserCommandValidator = new ActivateUserCommandValidator();
        }

        [Test]
        public async Task ThenFalseIsReturnedIfAllFieldsArentPopulated()
        {
            //Act
            var actual = await _activateUserCommandValidator.ValidateAsync(new ActivateUserCommand());

            //Assert
            Assert.IsAssignableFrom<ValidationResult>(actual);
            Assert.IsFalse(actual.IsValid());
        }


        [Test]
        public async Task ThenFalseIsReturnedIfNullIsPassed()
        {
            //Act
            var actual = await _activateUserCommandValidator.ValidateAsync(null);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedIfAccessCodeIsNull()
        {
            var command =
                new ActivateUserCommandBuilder()
                    .WithValidUser()
                    .WithNullAccessCode()
                    .Build();

            var result = await _activateUserCommandValidator.ValidateAsync(command);

            Assert.IsFalse(result.IsValid());
        }

        [TestCase("")]
        [TestCase("       ")]
        public async Task ThenFalseIsReturnedIfAccessCodeIsEmptyOrWhiteSpace(string accessCode)
        {
            var command =
                new ActivateUserCommandBuilder()
                    .WithValidUser()
                    .WithAccessCode(accessCode)
                    .Build();

            var result = await _activateUserCommandValidator.ValidateAsync(command);

            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public async Task ThenResultContainsMissingCodeMessageIfAccessCodeIsNull()
        {
            var command =
                new ActivateUserCommandBuilder()
                    .WithValidUser()
                    .WithNullAccessCode()
                    .Build();

            var result = await _activateUserCommandValidator.ValidateAsync(command);

            Assert.IsTrue(
                result
                    .ValidationDictionary.Values.Any(message => message.Equals(MissingCodeMessage, StringComparison.Ordinal))
            );
        }

        [TestCase("")]
        [TestCase("       ")]
        public async Task ThenResultContainsMissingCodeMessageIfAccessCodeIsEmptyOrWhiteSpace(string accessCode)
        {
            var command =
                new ActivateUserCommandBuilder()
                    .WithValidUser()
                    .WithAccessCode(accessCode)
                    .Build();

            var result = await _activateUserCommandValidator.ValidateAsync(command);


            Assert.IsTrue(
                result
                    .ValidationDictionary.Values.Any(message => message.Equals(MissingCodeMessage, StringComparison.Ordinal))
            );
        }

        [Test]
        public async Task ThenTrueIsReturnedIfAllFieldsAreProvidedAndTheAccessCodeMatchesCaseInsensitive()
        {
            //Act
            var command = new ActivateUserCommand
            {
                AccessCode = "AccessCode",
                UserId = Guid.NewGuid().ToString(),
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "ACCESSCODE",
                            CodeType = SecurityCodeType.AccessCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                }
            };
            var actual = await _activateUserCommandValidator.ValidateAsync(command);

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenResultContainsInvalidCodeMessageIfTheAccessCodeDoesntMatchTheAnyOnTheUser()
        {
            //Act
            var command = new ActivateUserCommand
            {
                AccessCode = "AccessCode",
                UserId = Guid.NewGuid().ToString(),
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "Edocssecca",
                            CodeType = SecurityCodeType.AccessCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                }
            };
            var result = await _activateUserCommandValidator.ValidateAsync(command);

            Assert.IsTrue(
                result
                    .ValidationDictionary.Values.Any(message => message.Equals(InvalidCodeMessage, StringComparison.Ordinal))
            );
        }

        [Test]
        public async Task ThenFalseIsReturnedIfTheAccessCodeDoesntMatchTheAnyOnTheUser()
        {
            //Act
            var command = new ActivateUserCommand
            {
                AccessCode = "AccessCode",
                UserId = Guid.NewGuid().ToString(),
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "Edocssecca",
                            CodeType = SecurityCodeType.AccessCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                }
            };
            var actual = await _activateUserCommandValidator.ValidateAsync(command);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedIfTheAccessCodeMatchAnyOnTheUserButItHasExpired()
        {
            //Act
            var command = new ActivateUserCommand
            {
                AccessCode = "AccessCode",
                UserId = Guid.NewGuid().ToString(),
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "AccessCode",
                            CodeType = SecurityCodeType.AccessCode,
                            ExpiryTime = DateTime.MinValue
                        }
                    }
                }
            };
            var actual = await _activateUserCommandValidator.ValidateAsync(command);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public async Task ThenTrueIsReturnedIfOnlyTheEmailHasBeenSuppliedAndTheUserIdIsNull()
        {
            //Act
            var command = new ActivateUserCommand
            {
                UserId = null,
                Email = "User@Email",
                User = new User
                {
                    Email = "user@email"
                }
            };
            var actual = await _activateUserCommandValidator.ValidateAsync(command);

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
