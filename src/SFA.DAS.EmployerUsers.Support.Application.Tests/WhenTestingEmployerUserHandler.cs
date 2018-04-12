using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerUsers.Support.Application.Handlers;
using SFA.DAS.EmployerUsers.Support.Core.Domain.Model;
using SFA.DAS.EmployerUsers.Support.Infrastructure;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerUsers.Support.Application.Tests
{
    [TestFixture]
    public class WhenTestingEmployerUserHandler
    {
        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILog>();
            _employerUserRepository = new Mock<IEmployerUserRepository>();
            _unit = new EmployerUserHandler(_employerUserRepository.Object, _mockLogger.Object);
        }

        private Mock<ILog> _mockLogger;
        private IEmployerUserHandler _unit;
        private Mock<IEmployerUserRepository> _employerUserRepository;


        [Test]
        public async Task ItShouldFindSearchItems()
        {
            _employerUserRepository.Setup(x => x.FindAllDetails(50,1)).Returns(
                Task.FromResult(
                    new List<EmployerUser>
                    {
                        new EmployerUser
                        {
                            Email = "Someone@tempuri.org", Accounts = new Collection<AccountDetailViewModel>(){}, AccountsUri = "~/account/{0}"
                        }
                    }.AsEnumerable()));

            var actual = await _unit.FindSearchItems(50, 1);
            CollectionAssert.IsNotEmpty(actual);
        }
    }
}