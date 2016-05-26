using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Infrastructure.UnitTests.ConfigurationTests.EmployerUsersClientComponentConfigurationFactoryTests
{
    public class WhenGettingConfiguration
    {
        private Mock<IConfigurationService> _configurationService;
        private EmployerUsersClientComponentConfigurationFactory _factory;

        [SetUp]
        public void Arrange()
        {
            _configurationService = new Mock<IConfigurationService>();
            _configurationService.Setup(s => s.GetAsync<EmployerUsersConfiguration>())
                .Returns(Task.FromResult<EmployerUsersConfiguration>(
                    new EmployerUsersConfiguration
                    {
                        IdentityServer = new IdentityServerConfiguration
                        {
                            ApplicationBaseUrl = "http://unittests.local/"
                        }
                    }));

            _factory = new EmployerUsersClientComponentConfigurationFactory(_configurationService.Object);
        }

        [Test]
        public void ThenItShouldReturnAccountActivationUrlPointingToConfirm()
        {
            // Act
            var actual = _factory.Get();

            // Assert
            Assert.AreEqual("http://unittests.local/account/confirm/", actual.AccountActivationUrl);
        }

        [Test]
        public void ThenItShouldThrowExceptionIfConfigurationServiceFails()
        {
            // Arrange
            _configurationService.Setup(s => s.GetAsync<EmployerUsersConfiguration>()).Throws<Exception>();

            // Act + Assert
            Assert.Throws<Exception>(() => _factory.Get());

        }
    }
}
