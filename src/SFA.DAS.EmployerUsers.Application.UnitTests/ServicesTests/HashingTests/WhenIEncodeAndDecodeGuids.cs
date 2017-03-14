using System;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Services.ValueHashing;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ServicesTests.HashingTests
{
    public class WhenIEncodeAndDecodeGuids
    {
        private HashingService _hashingService;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();

            _hashingService = new HashingService(new EmployerUsersConfiguration(), _logger.Object);
        }
        
        [Test]
        public void ThenTheValuesIsEncodedCorrecty()
        {
            //Arrange
            var expectedCode = "XJF88H6XHDKTNKH9JS4YF99T9XCBLS9JTLGCN4T86SGRSKY";

            //Act
            var actual = _hashingService.HashValue(new Guid("7c8dfc07-ecda-48ce-bf96-73b9fd37288f"));

            //Assert
            Assert.AreEqual(expectedCode,actual);
        }

        [Test]
        public void ThenTheValueCanBeDecodedToAGuid()
        {
            //Arrange
            var expectedGuid = new Guid("7c8dfc07-ecda-48ce-bf96-73b9fd37288f");

            //Act
            var actual = _hashingService.DecodeValue("XJF88H6XHDKTNKH9JS4YF99T9XCBLS9JTLGCN4T86SGRSKY");

            //Assert
            Assert.AreEqual(expectedGuid,actual);
        }

        [TestCase("XJF88H6XHDKTNKH9JS4YF999999999999T9XCBLS9JTLGCN4T86SGRSKY")]
        [TestCase("XJF88H6XHDKTNKH9JS4YF99T9XEBLS9JTLGCN4T99SGRSKY")]
        public void ThenAnAgrumentExceptionIsThrownIfTheValueCanNotBeDecoded(string value)
        {
            //Act
            var actual = _hashingService.DecodeValue(value);
            
            //Assert
            Assert.AreEqual(Guid.Empty,actual);
            _logger.Verify(x=>x.Warn(It.IsAny<ArgumentException>()), Times.Once);
        }
    }
}
