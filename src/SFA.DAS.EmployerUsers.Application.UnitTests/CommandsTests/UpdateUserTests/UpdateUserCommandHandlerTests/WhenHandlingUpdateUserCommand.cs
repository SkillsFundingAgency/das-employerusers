using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.UpdateUser;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Update;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.UpdateUserTests.UpdateUserCommandHandlerTests
{
    public class WhenHandlingUpdateUserCommand
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IAuditService> _auditService;
        private UpdateUserCommandHandler _handler;
        private UpdateUserCommand _command;

        [SetUp]
        public void Arrange()
        {
            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.GetById(It.IsAny<string>()))
                .ReturnsAsync(new Domain.User
                {
                    RequiresPasswordReset = false
                });

            _auditService = new Mock<IAuditService>();

            _handler = new UpdateUserCommandHandler(_userRepository.Object, _auditService.Object);

            _command = new UpdateUserCommand
            {
                User = new Domain.User
                {
                    Id = "User001",
                    RequiresPasswordReset = true
                }
            };
        }

        [Test]
        public async Task ThenItShouldUpdateTheUserInTheRepository()
        {
            // Act
            await _handler.Handle(_command);

            // Assert
            _userRepository.Verify(r => r.Update(_command.User), Times.Once);
        }

        [Test]
        public async Task ThenItShouldAuditTheUpdate()
        {
            // Act
            await _handler.Handle(_command);

            // Assert
            _auditService.Verify(s => s.WriteAudit(It.IsAny<UpdateUserAuditMessage>()), Times.Once);
        }
    }
}
