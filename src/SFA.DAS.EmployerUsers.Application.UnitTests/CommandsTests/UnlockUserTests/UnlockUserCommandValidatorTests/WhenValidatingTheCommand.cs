using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.UnlockUser;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.UnlockUserTests.UnlockUserCommandValidatorTests
{
    public class WhenValidatingTheCommand
    {
        private UnlockUserCommandValidator _unlockUserCommandValidator;

        [SetUp]
        public void Arrange()
        {
            _unlockUserCommandValidator = new UnlockUserCommandValidator();
        }

        [Test]
        public async Task ThenTheDictionaryIsPopulatedIfTheCommandIsNotPopulated()
        {
            //Act
            var actual = await _unlockUserCommandValidator.ValidateAsync(new UnlockUserCommand());

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public async Task ThenTheDicionaryIsEmptyIfTheCommmandIsPopulated()
        {
            //Act
            var actual = await
                _unlockUserCommandValidator.ValidateAsync(new UnlockUserCommand
                {
                    Email = "test@local",
                    UnlockCode = "SomeCode",
                    User = new User
                    {
                        SecurityCodes = new[]
                        {
                          new SecurityCode
                          {
                              Code = "SomeCode",
                              CodeType = SecurityCodeType.UnlockCode,
                              ExpiryTime = DateTime.UtcNow.AddMinutes(1)
                          }  
                        }
                    }
                });

            //Assert
            Assert.IsEmpty(actual.ValidationDictionary);
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenTheDictionaryIsNotEmptyIfTheUnlockCodesDoNotMatch()
        {
            //Act
            var actual = await
                _unlockUserCommandValidator.ValidateAsync(new UnlockUserCommand
                {
                    Email = "test@local",
                    UnlockCode = "SomeCode",
                    User = new User
                    {
                        SecurityCodes = new[]
                        {
                          new SecurityCode
                          {
                              Code = "AnotherCode",
                              CodeType = SecurityCodeType.UnlockCode,
                              ExpiryTime = DateTime.Now.AddMinutes(1)
                          }
                        }
                    }
                });

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("UnlockCodeMatch", "Unlock code is not correct"), actual.ValidationDictionary);
            Assert.IsFalse(actual.IsValid());
        }
        
        [Test]
        public async Task ThenTheDictionaryIsNotEmptyIfTheAccessCodeHasExpiredForAValidUnlockCode()
        {
            //Act
            var actual = await
                _unlockUserCommandValidator.ValidateAsync(new UnlockUserCommand
                {
                    Email = "test@local",
                    UnlockCode = "SomeCode",
                    User = new User
                    {
                        SecurityCodes = new[]
                        {
                          new SecurityCode
                          {
                              Code = "SomeCode",
                              CodeType = SecurityCodeType.UnlockCode,
                              ExpiryTime = DateTime.Now.AddMinutes(-1)
                          }
                        }
                    }
                });

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("UnlockCodeExpiry", "Unlock code has expired"), actual.ValidationDictionary);
            Assert.IsFalse(actual.IsValid());
        }
        
        [Test]
        public async Task ThenTheDictionaryContainsTheCorrectErrorMessagesWhenNotValid()
        {
            //Act
            var actual = await _unlockUserCommandValidator.ValidateAsync(new UnlockUserCommand());

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("User", "That account does not exist"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("Email", "Enter an email address"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("UnlockCode", "Enter an unlock code"), actual.ValidationDictionary);
            
        }
    }
}
