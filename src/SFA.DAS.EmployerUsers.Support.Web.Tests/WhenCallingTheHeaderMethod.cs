using System.Collections.Generic;
using System.IdentityModel;
using System.Threading.Tasks;
using System.Web.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Support.Core.Domain.Model;

namespace SFA.DAS.EmployerUsers.Support.Web.Tests
{


    [TestFixture]
    public class WhenCallingTheHeaderMethod : WhenTestingUserController
    {
        

        [Test]
        public async Task ItShouldReturnAViewOfTheMatchingEmployerUser()
        {
            _user = new EmployerUser { Email = $"{_id}@tempuri.org", Id = _id };
            _mockEmployerUserRepository.Setup(x => x.Get(_id)).Returns(Task.FromResult(_user));

            var actual = await _unit.Header(_id);

            Assert.IsInstanceOf<ViewResult>(actual);
            var viewResult = (ViewResult)actual;
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOf<EmployerUser>(viewResult.Model);
        }

       
        [Test]
        public async Task ItShouldReturnNotFoundIfTheIdInvalid()
        {
            _mockEmployerUserRepository.Setup(x => x.Get("234")).Returns(Task.FromResult(null as EmployerUser));

            var actual = await _unit.Header("234");

            Assert.IsInstanceOf<HttpNotFoundResult>(actual);
        }

        [Test]
        public void ItShouldThrowABadRequestExceptionIfTheIdIsNull()
        {
            Assert.ThrowsAsync<BadRequestException>(async () => await _unit.Header(null));
        }

        [Test]
        public void ItShouldThrowABadRequestExceptionIfTheIdIsWhiteSpace()
        {
            Assert.ThrowsAsync<BadRequestException>(async () => await _unit.Header(" "));
        }
    }
}