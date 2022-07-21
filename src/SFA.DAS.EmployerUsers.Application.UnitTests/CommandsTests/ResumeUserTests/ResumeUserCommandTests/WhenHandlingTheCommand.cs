using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Commands.ResumeUser;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Suspend;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ResumeUserTests.ResumeUserCommandTests
{
    public class WhenHandlingTheCommand
    {
        private ResumeUserCommandHandler _handler;
        private Mock<IUserRepository> _userRepository;
        private Mock<IAuditService> _auditService;

        [SetUp]
        public void Arrange()
        {
            _userRepository = new Mock<IUserRepository>();
            _auditService = new Mock<IAuditService>();
            _handler = new ResumeUserCommandHandler(_userRepository.Object, _auditService.Object);

        }

        [Test, AutoData]
        public async Task ThenTheUserRepositoryIsCalledIfTheCommandIsValid(User user, ChangedByUserInfo changedByUserInfo)
        {
            _auditService.Setup(a => a.WriteAudit(It.Is<ResumeUserAuditMessage>(m => m.Category == "UPDATE" && m.Description == $"User {user.Email} (id: {user.Id}) has been re-activated after suspension by {changedByUserInfo.Email} (id: {changedByUserInfo.UserId})")))
                .Returns(Task.CompletedTask);

            _userRepository.Setup(x => x.Resume(It.Is<User>(u => u.Id == user.Id))).Returns(Task.CompletedTask);

            var command = new ResumeUserCommand(user, changedByUserInfo);

            //Act
            await _handler.Handle(command);

            //Assert
            _userRepository.Verify(x => x.Resume(It.Is<User>(u => u.Id == user.Id)), Times.Once);
            _auditService.Verify(x => x.WriteAudit(It.Is<ResumeUserAuditMessage>(m => m.Category == "UPDATE" && m.Description == $"User {user.Email} (id: {user.Id}) has been re-activated after suspension by {changedByUserInfo.Email} (id: {changedByUserInfo.UserId})")), Times.Once);

        }
      
    }
}
