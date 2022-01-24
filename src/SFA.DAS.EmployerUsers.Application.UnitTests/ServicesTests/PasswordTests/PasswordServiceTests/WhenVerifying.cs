using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.PasswordTests.PasswordServiceTests
{
    public class WhenVerifying
    {
        private const string CorrectPassword = "password";
        private const string IncorrectPassword = "password1";
        private const string CorrectHashedPassword = "azYot9HLQpd7lBccbJtvV8rqeJdDJ7YuJVfty2V2H9BJE446ove5AXYwvWY70V/yQm0MKh7MAhd/bNgVc8zb5ThqwFp8OWHEOEDx/grhhlphXKaI0KqZrlxOHConhI+Qop0aowpB3+7CsVqsRbrT637BRdt2LDQg/P92K3sucQQfVDN/crrIUxLzxWEWoNQ3zUpcXoCfghe4Hulz6A6lKgTrBlEVbdJhTuGSb/0nAP1sS4HXj2H66CGRjXYpEA2X2pPhJuQk7Os04AYxey3AUz3UgTvmFFzEFxzG5ugbqrx1TE5wBYHuVo6cVcCp93+v79oo7eB8lrqKeNmzOOtQCA==";
        private const string IncorrectHashedPassword = "0rkuhDIxgQbUyrskfU4uAnLaFGzBl7ZD/ihofMsX7nJ0bziW2rynXZlbMKfxFv/eKqf2MyFUAUW1VCXuz9w82uPT02cDGRXHFSgSu9F2dJkT7XOnXCXc6uMOLqDHffZLol2AXPKhEv+K1KWfZTiJPr/PVEKVQD/+60IGvw7vTcjBAilOohRpyyWotux9MlPp8l3RyUuz9HbPQrUZp3m+ClxIomAtKRW2Hk2ojdiNHs6mKJATBlzej/ABRnmqTdxlvns4v6o4pFAFLQn9GXUrSdYmTHGJdcK3iYJk5H3NAcvWV/YI5RxTLdj7QRGlyBiVSBGSdSV8XNIL58i1p2KEtA==";
        private const string CorrectSalt = "H9neDfu0bUWWjHa2XPL9/w==";
        private const string IncorrectSalt = "YmxhaGJsYWg=";
        private const string CorrectProfileId = "XYZ";
        private const string IncorrectProfileId = "ABC";

        private Mock<IPasswordProfileRepository> _passwordProfileRepo;
        private PasswordService _passwordService;

        [SetUp]
        public void Arrange()
        {
            var configuration = new EmployerUsersConfiguration
            {
                Account = new AccountConfiguration
                {
                    ActivePasswordProfileId = "XYZ"
                }
            };

            _passwordProfileRepo = new Mock<IPasswordProfileRepository>();
            _passwordProfileRepo.Setup(r => r.GetAllAsync()).Returns(Task.FromResult<IEnumerable<PasswordProfile>>(new[]
            {
                new PasswordProfile
                {
                    Id = "XYZ",
                    Key = Convert.ToBase64String(Encoding.Unicode.GetBytes("SystemKey")),
                    WorkFactor = 10,
                    SaltLength = 16,
                    StorageLength = 256
                },

                new PasswordProfile
                {
                    Id = "ABC",
                    Key = Convert.ToBase64String(Encoding.Unicode.GetBytes("SystemKey")),
                    WorkFactor = 9,
                    SaltLength = 16,
                    StorageLength = 256
                }
            }));

            _passwordService = new PasswordService(configuration, _passwordProfileRepo.Object);
        }

        [TestCase(IncorrectPassword, CorrectHashedPassword, CorrectSalt, CorrectProfileId)]
        [TestCase(CorrectPassword, IncorrectHashedPassword, CorrectSalt, CorrectProfileId)]
        [TestCase(CorrectPassword, CorrectHashedPassword, IncorrectSalt, CorrectProfileId)]
        [TestCase(CorrectPassword, CorrectHashedPassword, CorrectSalt, IncorrectProfileId)]
        public async Task ThenItReturnsFalseWhenThePasswordIsIncorrect(string plainTextPassword, string hashedPassword, string salt, string profileId)
        {
            // Act
            var actual = await _passwordService.VerifyAsync(plainTextPassword, hashedPassword, salt, profileId);

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public async Task ThenItReturnsTrueWhenThePasswordIsCorrect()
        {
            // Act
            var actual = await _passwordService.VerifyAsync(CorrectPassword, CorrectHashedPassword, CorrectSalt, CorrectProfileId);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestCase("")]
        [TestCase(null)]
        public void ThenItThrowsArgumentNullExceptionWhenPlainTextPasswordMissing(string plainPassword)
        {
            // Act + Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _passwordService.VerifyAsync(plainPassword, CorrectHashedPassword, CorrectSalt, CorrectProfileId));
            Assert.AreEqual("plainTextPassword", ex.ParamName);
        }

        [TestCase("")]
        [TestCase(null)]
        public void ThenItThrowsArgumentNullExceptionWhenHashedPasswordMissing(string hashedPassword)
        {
            // Act + Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _passwordService.VerifyAsync(CorrectPassword, hashedPassword, CorrectSalt, CorrectProfileId));
            Assert.AreEqual("hashedPassword", ex.ParamName);
        }

        [TestCase("")]
        [TestCase(null)]
        public void ThenItThrowsArgumentNullExceptionWhenSaltMissing(string salt)
        {
            // Act + Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _passwordService.VerifyAsync(CorrectPassword, CorrectHashedPassword, salt, CorrectProfileId));
            Assert.AreEqual("salt", ex.ParamName);
        }

        [TestCase("")]
        [TestCase(null)]
        public void ThenItThrowsArgumentNullExceptionWhenProfileIdMissing(string profileId)
        {
            // Act + Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _passwordService.VerifyAsync(CorrectPassword, CorrectHashedPassword, CorrectSalt, profileId));
            Assert.AreEqual("profileId", ex.ParamName);
        }
    }
}
