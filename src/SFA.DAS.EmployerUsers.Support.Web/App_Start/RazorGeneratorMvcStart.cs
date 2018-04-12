using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;
using System.Diagnostics.CodeAnalysis;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(SFA.DAS.EmployerUsers.Support.Web.RazorGeneratorMvcStart), "Start")]

namespace SFA.DAS.EmployerUsers.Support.Web {
    [ExcludeFromCodeCoverage]
    public static class RazorGeneratorMvcStart {
        public static void Start() {
            var engine = new PrecompiledMvcEngine(typeof(RazorGeneratorMvcStart).Assembly) {
                UsePhysicalViewsIfNewer = HttpContext.Current.Request.IsLocal
            };

            ViewEngines.Engines.Insert(0, engine);

            // StartPage lookups are done by WebPages. 
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}
