using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Castle.Components.DictionaryAdapter;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerUsers.Support.Core.Domain.Model;

namespace SFA.DAS.EmployerUsers.Support.Web.Tests
{
    [TestFixture]
    public class WhenCallingTheIndexMethod : WhenTestingUserController
    {
        private readonly ICollection<AccountDetailViewModel> _accountsList = new Collection<AccountDetailViewModel>()
        {
            new AccountDetailViewModel(){ AccountId = 1, HashedAccountId = "ASDAS", Balance = 0.0m, DateRegistered = DateTime.UtcNow, DasAccountName = "Account Name 1", OwnerEmail = "owner@account.org"},
            new AccountDetailViewModel(){ AccountId = 2, HashedAccountId = "ASDAT", Balance = 0.0m, DateRegistered = DateTime.UtcNow, DasAccountName = "Account Name 2", OwnerEmail = "owner@account.org"}
        };
        [Test]
        public async Task ItShouldReturnAViewOfTheMatchingEmployerUser()
        {
            _user = new EmployerUser {Email = $"{_id}@tempuri.org", Id = _id};
            _mockEmployerUserRepository.Setup(x => x.Get(_id)).Returns(Task.FromResult(_user));
            _mockEmployerUserRepository.Setup(x => x.GetAccounts(_id)).ReturnsAsync(_accountsList);

            var actual = await _unit.Index(_id);

            Assert.IsInstanceOf<ViewResult>(actual);
            var viewResult = (ViewResult) actual;
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOf<EmployerUser>(viewResult.Model);
            Assert.NotNull(((EmployerUser)viewResult.Model).Accounts);
            Assert.AreEqual(_accountsList.Count, ((EmployerUser)viewResult.Model).Accounts.Count());
        }
       

        [Test]
        public async Task ItShouldReturnNotFoundIfTheIdInvalid()
        {
            _mockEmployerUserRepository.Setup(x => x.Get("234")).Returns(Task.FromResult(null as EmployerUser));

            var actual = await _unit.Index("234");

            Assert.IsInstanceOf<HttpNotFoundResult>(actual);
        }

        [Test]
        public void ItShouldThrowABadRequestExceptionIfTheIdIsNull()
        {
            Assert.ThrowsAsync<BadRequestException>(async () => await _unit.Index(null));
        }

        [Test]
        public void ItShouldThrowABadRequestExceptionIfTheIdIsWhiteSpace()
        {
            Assert.ThrowsAsync<BadRequestException>(async () => await _unit.Index(" "));
        }
    }
}