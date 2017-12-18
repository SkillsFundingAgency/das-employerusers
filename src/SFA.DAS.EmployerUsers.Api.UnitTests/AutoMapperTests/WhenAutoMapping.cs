using AutoMapper;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.DependencyResolution;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.AutoMapperTests
{
    [TestFixture]
    public class WhenAutoMapping
    {
        private MapperConfiguration _config;

        [SetUp]
        public void Arrange()
        {
            _config = new MapperConfiguration(c =>
            {
                c.AddProfile<DefaultProfile>();
            });
        }

        [Test]
        public void ThenAutoMappingShouldBeCorrect()
        {
            _config.AssertConfigurationIsValid();
        }
    }
}