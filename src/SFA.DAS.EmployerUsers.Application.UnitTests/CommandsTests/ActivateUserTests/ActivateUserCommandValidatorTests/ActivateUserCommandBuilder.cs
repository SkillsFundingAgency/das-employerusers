using System;
using System.Diagnostics.CodeAnalysis;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ActivateUserTests.ActivateUserCommandValidatorTests
{
    [ExcludeFromCodeCoverage]
    public class ActivateUserCommandBuilder
    {
        private string _accessCode = "AccessCode";
        private string _userId = Guid.NewGuid().ToString();
        private User _user;

        public ActivateUserCommandBuilder WithAccessCode(string code)
        {
            _accessCode = code;

            return this;
        }

        public ActivateUserCommandBuilder WithValidUser()
        {
            _user = new User
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
            };

            return this;
        }


        public ActivateUserCommandBuilder WithNullAccessCode()
        {
            _accessCode = null;

            return this;
        }

        public ActivateUserCommand Build()
        {
            return new ActivateUserCommand
            {
                AccessCode = _accessCode,
                UserId = _userId,
                User = _user
            };
        }
    }
}