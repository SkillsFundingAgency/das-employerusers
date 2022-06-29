using System.Threading.Tasks;
using System.Web.Http.Results;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Commands.ResumeUser;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.UserControllerTests
{
    [TestFixture]
    public class WhenIResumeAUser : UserControllerTestsBase
    {
        [Test, AutoData]
        public async Task ThenNoErrorIsReturned_WhenTheRequestIsValid(ChangedByUserInfo changedByUserInfo)
        {
            var userId = "ABC123";
            var userResponse = new User()
            {
                Id = userId,
                Email = "t@t.com",
                FirstName = "test",
                LastName = "user",
                IsActive = true,
                IsLocked = false,
                IsSuspended = false,
            };

            Mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId))).ReturnsAsync(userResponse);
            Mediator.Setup(x => x.SendAsync(It.Is<ResumeUserCommand>(c => c.User != null && c.User.Id == userId))).ReturnsAsync(new Unit());

            var response = await Controller.Resume(userId, changedByUserInfo);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ResumeUserResponse>>(response);
            var model = response as OkNegotiatedContentResult<ResumeUserResponse>;

            model?.Content.Should().NotBeNull();
            model?.Content.Id.Should().Be(userId);
            
            Mediator.Verify(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId)), Times.Once);
            Mediator.Verify(x => x.SendAsync(It.Is<ResumeUserCommand>(c => c.User != null && c.User.Id == userId)), Times.Once);
        }

        [Test, AutoData]
        public async Task AndTheUserDoesNotExist_ThenANotFoundResultIsReturned(ChangedByUserInfo changedByUserInfo)
        {
            var userId = "ABC123";

            Mediator.Setup(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId)))
                .ReturnsAsync((User)null);
            Mediator.Setup(x => x.SendAsync(It.Is<ResumeUserCommand>(c => c.User != null && c.User.Id == userId)))
                .ReturnsAsync(new Unit());

            var response = await Controller.Suspend(userId, changedByUserInfo);

            Assert.IsNotNull(response);
            response.Should().BeOfType<NotFoundResult>();

            Mediator.Verify(x => x.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == userId)), Times.Once);
            Mediator.Verify(x => x.SendAsync(It.Is<ResumeUserCommand>(c => c.User != null && c.User.Id == userId)), Times.Never);
        }
    }
}
