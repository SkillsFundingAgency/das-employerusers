using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ResumeUser;
using SFA.DAS.EmployerUsers.Application.Commands.SuspendUser;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Suspend;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ResumeUserTests.ResumeUserCommandTests
{
    public class WhenHandlingTheCommand
    {
        private const string UserId = "ABC123";

        private User _user;
        private ResumeUserCommandHandler _handler;
        private Mock<IUserRepository> _userRepository;
        private ResumeUserCommand _command;
        private Mock<ILogger> _logger;
        private Mock<IAuditService> _auditService;

        [SetUp]
        public void Arrange()
        {
            _user = new User()
            {
                Id = UserId,
                Email = "x@y.com"
            };

            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(x => x.Resume(It.Is<User>(u => u.Id == UserId))).Returns(Task.CompletedTask);
            _auditService = new Mock<IAuditService>();
            _logger = new Mock<ILogger>();
            _handler = new ResumeUserCommandHandler(_userRepository.Object, _auditService.Object);
            
            _command = new ResumeUserCommand
            {
                User = _user
            };
        }

        [Test]
        public async Task ThenTheUserRepositoryIsCalledIfTheCommandIsValid()
        {
            _auditService.Setup(a => a.WriteAudit(It.Is<ResumeUserAuditMessage>(m => m.Category == "UPDATE" && m.Description == $"User {_user.Email} (id: {_user.Id}) has been re-activated after suspension")))
                .Returns(Task.CompletedTask);

            //Act
            await _handler.Handle(_command);

            //Assert
            _userRepository.Verify(x => x.Resume(It.Is<User>(u => u.Id == UserId)), Times.Once);
            _auditService.Verify(x => x.WriteAudit(It.Is<ResumeUserAuditMessage>(m => m.Category == "UPDATE" && m.Description == $"User {_user.Email} (id: {_user.Id}) has been re-activated after suspension")), Times.Once);

        }
      
    }
}
