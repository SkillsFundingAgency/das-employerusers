using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Types;

namespace SFA.DAS.EmployerUsers.Api.Client.UnitTests.EmployerUsersApiClientTests
{
    public class WhenSettingUserToRequirePasswordReset : EmployerUsersApiClientTestsBase
    {
        [Test]
        public async Task ThenItShouldPatchUserWithRequiresPasswordReset()
        {
            // Act
            await Client.SetUserToRequirePasswordReset("User001");

            // Assert
            HttpClient.Verify(c => c.PatchAsync("http://some-url/api/users/User001", It.Is<PatchUserViewModel>(m => m.RequiresPasswordReset == true)), Times.Once);
        }
    }
}
