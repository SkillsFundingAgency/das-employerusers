using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.DeleteUser;
using SFA.DAS.EmployerUsers.Application.Queries.GetUsersWithExpiredRegistrations;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.RegistrationTidyUpJob.RegistrationManagement;

namespace SFA.DAS.EmployerUsers.RegistrationTidyUpJob.UnitTests.RegistrationManagementTests.RegistrationManagerTests
{
    public class WhenRemovingExpiredRegistrations
    {
        private User _user1;
        private User _user2;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private RegistrationManager _manager;

        [SetUp]
        public void Arrange()
        {
            _user1 = new User
            {
                Id = "USER1"
            };
            _user2 = new User
            {
                Id = "USER2"
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetUsersWithExpiredRegistrationsQuery>()))
                .ReturnsAsync(new[] { _user1, _user2 });

            _logger = new Mock<ILogger>();

            _manager = new RegistrationManager(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenItShouldDeleteAllUsersWithExpiredRegistrations()
        {
            // Act
            await _manager.RemoveExpiredRegistrations();

            // Assert
            _mediator.Verify(m => m.SendAsync(It.IsAny<DeleteUserCommand>()), Times.Exactly(2));
            _mediator.Verify(m => m.SendAsync(It.Is<DeleteUserCommand>(c => c.User == _user1)), Times.Once);
            _mediator.Verify(m => m.SendAsync(It.Is<DeleteUserCommand>(c => c.User == _user2)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldDoNothingWhenThereAreNoUsersWithExpiredRegistrations()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetUsersWithExpiredRegistrationsQuery>()))
                .ReturnsAsync(new User[0]);

            // Act
            await _manager.RemoveExpiredRegistrations();

            // Assert
            _mediator.Verify(m => m.SendAsync(It.IsAny<DeleteUserCommand>()), Times.Never);
        }
    }
}
