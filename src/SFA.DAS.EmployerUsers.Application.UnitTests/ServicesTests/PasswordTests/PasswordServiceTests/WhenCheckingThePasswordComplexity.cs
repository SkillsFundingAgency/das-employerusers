using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.PasswordTests.PasswordServiceTests
{
    public class WhenCheckingThePasswordComplexity
    {
        private PasswordService _passwordService;
        private Mock<IConfigurationService> _configurationService;
        private Mock<IPasswordProfileRepository> _passwordProfileRepo;

        [SetUp]
        public void Arrange()
        {
            _configurationService = new Mock<IConfigurationService>();
            _passwordProfileRepo = new Mock<IPasswordProfileRepository>();

            _passwordService = new PasswordService(_configurationService.Object, _passwordProfileRepo.Object);
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
