using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Commands.SuspendUser;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Suspend;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.SuspendUserTests.SuspendUserCommandTests
{
    public class WhenHandlingTheCommand
    {
        private const string UserId = "ABC123";

        private User _user;
        private SuspendUserCommandHandler _handler;
        private Mock<IUserRepository> _userRepository;
        private SuspendUserCommand _command;
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
            _userRepository.Setup(x => x.Suspend(It.Is<User>(u => u.Id == UserId))).Returns(Task.CompletedTask);
            _auditService = new Mock<IAuditService>();
            _logger = new Mock<ILogger>();
            _handler = new SuspendUserCommandHandler(_userRepository.Object, _auditService.Object);
            
            _command = new SuspendUserCommand
            {
                User = _user
            };
        }

        [Test]
        public async Task ThenTheUserRepositoryIsCalledIfTheCommandIsValid()
        {
            _auditService.Setup(a => a.WriteAudit(It.Is<SuspendUserAuditMessage>(m => m.Category == "UPDATE" && m.Description == $"User { _user.Email} (id: { _user.Id}) has been suspended")))
                .Returns(Task.CompletedTask);

            //Act
            await _handler.Handle(_command);

            //Assert
            _userRepository.Verify(x => x.Suspend(It.Is<User>(u => u.Id == UserId)), Times.Once);
            _auditService.Verify(x => x.WriteAudit(It.Is<SuspendUserAuditMessage>(m => m.Category == "UPDATE" && m.Description == $"User { _user.Email} (id: { _user.Id}) has been suspended")), Times.Once);

        }
      
    }
}
