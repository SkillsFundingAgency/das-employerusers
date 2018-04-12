using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerUsers.Api.Client;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerUsers.Support.Infrastructure.Tests
{
    [TestFixture]
    public class WhenTestingEmployerUserRepository
    {
        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILog>();
            _employerUserApiClient = new Mock<IEmployerUsersApiClient>();
            _employerAccountApiClient = new Mock<IAccountApiClient>();
            _unit = new EmployerUserRepository(_logger.Object, _employerUserApiClient.Object, _employerAccountApiClient.Object);
        }

        private IEmployerUserRepository _unit;
        private Mock<ILog> _logger;
        private Mock<IEmployerUsersApiClient> _employerUserApiClient;
        private Mock<IAccountApiClient> _employerAccountApiClient;
        private readonly string _id = "123";

        [Test]
        public async Task ItShouldRetrieveAllOfTheAvailableUsers()
        {
            var firstPage = new PagedApiResponseViewModel<UserSummaryViewModel>
            {
                TotalPages = 2,
                Page = 1,
                Data = new List<UserSummaryViewModel> {new UserSummaryViewModel()}
            };


            _employerUserApiClient
                .Setup(x =>x.GetPageOfEmployerUsers(1, 50))
                .Returns(Task.FromResult(firstPage));

            var actual = await _unit.FindAllDetails(50,1);

            _employerUserApiClient
                .Verify(x => x.GetPageOfEmployerUsers(1, 50), Times.Once);

            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public async Task ItShouldReturnANullUserifTheRequestedUserDoesNotExist()
        {
            _employerUserApiClient
                .Setup(x => x.GetResource<UserViewModel>($"/api/users/{_id}"))
                .Returns(Task.FromResult(null as UserViewModel));

            var actual = await _unit.Get(_id);
            Assert.IsNull(actual);
        }

        [Test]
        public async Task ItShouldReturnTheRequestedUser()
        {
            _employerUserApiClient
                .Setup(x => x.GetResource<UserViewModel>($"/api/users/{_id}"))
                .Returns(Task.FromResult(new UserViewModel {Id = _id}));

            var actual = await _unit.Get(_id);
            Assert.IsNotNull(actual);
        }
    }
}