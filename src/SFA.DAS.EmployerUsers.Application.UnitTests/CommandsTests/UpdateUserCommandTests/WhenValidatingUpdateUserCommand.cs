using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.UpdateUser;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.UpdateUserCommandTests
{
    public class WhenValidatingUpdateUserCommand
    {
        private UpdateUserCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new UpdateUserCommandValidator();
        }

        [Test]
        public async Task Then_Not_Valid_If_No_Values()
        {
            var command = new UpdateUserCommand();

            var actual = await _validator.ValidateAsync(command);
            
            Assert.IsFalse(actual.IsValid());
        }
        [Test]
        public async Task Then_Not_Valid_If_No_Email()
        {
            var command = new UpdateUserCommand
            {
                GovUkIdentifier = "some-identifier"
            };

            var actual = await _validator.ValidateAsync(command);
            
            Assert.IsFalse(actual.IsValid());
        }
       
        [Test]
        public async Task Then_Not_Valid_If_No_GovUkIdentifier()
        {
            var command = new UpdateUserCommand
            {
                Email = "some-value"
            };

            var actual = await _validator.ValidateAsync(command);
            
            Assert.IsFalse(actual.IsValid());
        } 
        
        [Test]
        public async Task Then_Valid_If_All_Values_Supplied()
        {
            var command = new UpdateUserCommand
            {
                Email = "some-value",
                GovUkIdentifier = "some-identifier"
            };

            var actual = await _validator.ValidateAsync(command);
            
            Assert.IsTrue(actual.IsValid());
        } 
    }
}