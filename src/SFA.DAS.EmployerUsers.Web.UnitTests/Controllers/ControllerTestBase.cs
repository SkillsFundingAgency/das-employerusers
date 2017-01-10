using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Moq;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers
{
    public abstract class ControllerTestBase
    {
        protected Mock<HttpRequestBase> _httpRequest;
        protected Mock<HttpContextBase> _httpContext;
        protected Mock<ControllerContext> _controllerContext;

        public virtual void Arrange()
        {
            _httpRequest = new Mock<HttpRequestBase>();
            _httpRequest.Setup(r => r.UserHostAddress).Returns("123.123.123.123");
            _httpRequest.Setup(r => r.Url).Returns(new Uri("https://localhost"));
            _httpRequest.Setup(r => r.Cookies).Returns(new HttpCookieCollection());

            _httpContext = new Mock<HttpContextBase>();
            _httpContext.Setup(c => c.Request).Returns(_httpRequest.Object);

            _controllerContext = new Mock<ControllerContext>();
            _controllerContext.Setup(c => c.HttpContext).Returns(_httpContext.Object);
        }

        protected void AddUserToContext(string id = "USER_ID", string email = "my@local.com")
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(DasClaimTypes.Id, id),
                new Claim(DasClaimTypes.Email, email)
            },"TestAuth");
            
            
            var principal = new ClaimsPrincipal(identity);
            
            _httpContext.Setup(c => c.User).Returns(principal);
        }
    }
}
