using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.PasswordTests.PasswordServiceTests
{
    public class WhenGenerating
    {
        private Mock<IPasswordProfileRepository> _passwordProfileRepo;
        private PasswordService _passwordService;

        [SetUp]
        public void Arrange()
        {
            _passwordProfileRepo = new Mock<IPasswordProfileRepository>();
            _passwordProfileRepo.Setup(r => r.GetAllAsync()).Returns(Task.FromResult<IEnumerable<PasswordProfile>>(new[]
            {
                new PasswordProfile
                {
                    Id = "XXX",
                    Key = Convert.ToBase64String(Encoding.Unicode.GetBytes("SystemKey")),
                    WorkFactor = 1,
                    SaltLength = 16,
                    StorageLength = 256
                }
            }));

            _passwordService = new PasswordService(_passwordProfileRepo.Object);
        }

        [Test]
        public async Task ThenItReturnsASecuredPasswordInstance()
        {
            // Act
            var actual = await _passwordService.GenerateAsync("password");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItReturnsSecurePasswordDetails()
        {
            // Act
            var actual = await _passwordService.GenerateAsync("password");

            // Assert
            Assert.IsNotEmpty(actual.HashedPassword);
            Assert.IsNotEmpty(actual.Salt);
            Assert.AreEqual("XXX", actual.ProfileId);
        }
    }
}
