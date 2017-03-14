using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByHashedId;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.QueriesTests.GetUserByHashedIdTests
{
    public class WhenIValidateTheRequest
    {
        private GetUserByHashedIdValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetUserByHashedIdValidator();
        }

        [Test]
        public async Task WhenAllFieldsArePopulatedTrueIsReturned()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetUserByHashedIdQuery {HashedUserId = "123AFS"});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task WhenTheFieldsAreNotPopulatedThenTheErrorDictionaryIsPopulatedAndFalseReturned()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetUserByHashedIdQuery());

            //Act
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("HashedUserId", "HashedUserId has not been supplied"),actual.ValidationDictionary );
        }
    }
}