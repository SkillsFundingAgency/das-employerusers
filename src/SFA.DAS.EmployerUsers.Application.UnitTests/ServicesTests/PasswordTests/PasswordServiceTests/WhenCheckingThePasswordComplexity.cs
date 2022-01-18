using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.PasswordTests.PasswordServiceTests
{
    public class WhenCheckingThePasswordComplexity
    {
        private PasswordService _passwordService;
        private Mock<IPasswordProfileRepository> _passwordProfileRepo;

        [SetUp]
        public void Arrange()
        {
            var configuration = new EmployerUsersConfiguration();
            _passwordProfileRepo = new Mock<IPasswordProfileRepository>();

            _passwordService = new PasswordService(configuration, _passwordProfileRepo.Object);
        }

        [TestCase("Passw0r")]
        [TestCase("Password")]
        [TestCase("123456789")]
        [TestCase("aaaaa6789")]
        [TestCase("AAAAA6789")]
        [TestCase("")]
        [TestCase(null)]
        public void ThenFalseIsReturnedIfThePasswordDoesNotTheRequiredStrenth(string password)
        {
            //Act
            var actual = _passwordService.CheckPasswordMatchesRequiredComplexity(password);

            //Assert
            Assert.IsFalse(actual);
        }
    }
}
