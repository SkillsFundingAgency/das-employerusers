using System.Web;
using System.Web.Mvc;
using Moq;

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

            _httpContext = new Mock<HttpContextBase>();
            _httpContext.Setup(c => c.Request).Returns(_httpRequest.Object);

            _controllerContext = new Mock<ControllerContext>();
            _controllerContext.Setup(c => c.HttpContext).Returns(_httpContext.Object);
        }
    }
}
