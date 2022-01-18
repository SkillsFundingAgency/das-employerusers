using System.Diagnostics.CodeAnalysis;
using System.Web;
using SFA.DAS.EmployerUsers.Support.Web.Logging;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EmployerUsers.Support.Web.DependencyResolution
{
    [ExcludeFromCodeCoverage]
    public class WebRegistry : Registry
    {
        public WebRegistry()
        {
            For<IRequestContext>().Use(x => new RequestContext(new HttpContextWrapper(HttpContext.Current)));
        }
    }
}