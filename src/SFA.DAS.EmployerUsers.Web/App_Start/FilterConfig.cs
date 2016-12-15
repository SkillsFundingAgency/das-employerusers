using System.Web.Mvc;
using SFA.DAS.EmployerUsers.Web.Plumbing.Mvc;

namespace SFA.DAS.EmployerUsers.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogAndHandleErrorAttribute());
            filters.Add(new RequireHttpsAttribute());
        }
    }
}
