using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByHashedId;
using SFA.DAS.HashingService;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.QueriesTests.GetUserByHashedIdTests
{
    public class WhenIGetUserByHashedId
    {
        private GetUserByHashedIdQueryHandler _handler;
        private Mock<IHashingService> _hashingService;
        private Mock<IValidator<GetUserByHashedIdQuery>> _validator;
        private const string HashedUserId = "123ADF345";
        private Guid _expectedUserId;
        private Mock<IUserRepository> _userRepositry;

        [SetUp]
        public void Arrange()
        {
            _expectedUserId = Guid.NewGuid();

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValueToGuid(HashedUserId)).Returns(_expectedUserId);

            _validator = new Mock<IValidator<GetUserByHashedIdQuery>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetUserByHashedIdQuery>())).ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            _userRepositry = new Mock<IUserRepository>();
            _userRepositry.Setup(x => x.GetById(_expectedUserId.ToString())).ReturnsAsync(new User());

            _handler = new GetUserByHashedIdQueryHandler(_hashingService.Object, _validator.Object, _userRepositry.Object);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _handler.Handle(new GetUserByHashedIdQuery());

            //Assert
            _validator.Verify(x=>x.ValidateAsync(It.IsAny<GetUserByHashedIdQuery>()), Times.Once);
        }

        [Test]
        public void ThenAnInvalidRequestExceptionIsThrownIfValueIsMissing()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetUserByHashedIdQuery>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { {"",""} } });

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(new GetUserByHashedIdQuery()));
        }

        [Test]
        public async Task ThenIfTheHahsedUserIdIsNotValidThenANullUserIsReturned()
        {
            //Act
            var actual = await _handler.Handle(new GetUserByHashedIdQuery {HashedUserId = "123RFD" });

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task ThenTheUserIsSearchedForById()
        {
            //Act
            var actual = await _handler.Handle(new GetUserByHashedIdQuery { HashedUserId = HashedUserId});

            //Assert
            Assert.IsNotNull(actual);
            _userRepositry.Verify(x=>x.GetById(_expectedUserId.ToString()), Times.Once);
        }

        [Test]
        public async Task ThenTheUserIsReturnedInTheResponseIfFound()
        {
            //Act
            var actual = await _handler.Handle(new GetUserByHashedIdQuery { HashedUserId = HashedUserId });

            //Assert
            Assert.IsAssignableFrom<User>(actual);
        }
    }
}
