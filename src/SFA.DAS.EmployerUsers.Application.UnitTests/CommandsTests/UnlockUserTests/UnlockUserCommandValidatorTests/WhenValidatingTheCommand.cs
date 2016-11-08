using System;
using System.Collections.Generic;
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
        public void ThenTheDictionaryIsPopulatedIfTheCommandIsNotPopulated()
        {
            //Act
            var actual = _unlockUserCommandValidator.Validate(new UnlockUserCommand());

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenTheDicionaryIsEmptyIfTheCommmandIsPopulated()
        {
            //Act
            var actual =
                _unlockUserCommandValidator.Validate(new UnlockUserCommand
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
        public void ThenTheDictionaryIsNotEmptyIfTheUnlockCodesDoNotMatch()
        {
            //Act
            var actual =
                _unlockUserCommandValidator.Validate(new UnlockUserCommand
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
            Assert.Contains(new KeyValuePair<string, string>("UnlockCodeMatch", "Unlock Code is not correct"), actual.ValidationDictionary);
            Assert.IsFalse(actual.IsValid());
        }
        
        [Test]
        public void ThenTheDictionaryIsNotEmptyIfTheAccessCodeHasExpiredForAValidUnlockCode()
        {
            //Act
            var actual =
                _unlockUserCommandValidator.Validate(new UnlockUserCommand
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
            Assert.Contains(new KeyValuePair<string, string>("UnlockCodeExpiry", "Unlock Code has expired"), actual.ValidationDictionary);
            Assert.IsFalse(actual.IsValid());
        }
        
        [Test]
        public void ThenTheDictionaryContainsTheCorrectErrorMessagesWhenNotValid()
        {
            //Act
            var actual = _unlockUserCommandValidator.Validate(new UnlockUserCommand());

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("User", "User Does Not Exist"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("Email", "Email has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("UnlockCode", "Unlock Code has not been supplied"), actual.ValidationDictionary);
            
        }
    }
}
