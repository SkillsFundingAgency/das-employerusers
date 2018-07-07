using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SFA.DAS.Support.Shared.Navigation;

namespace SFA.DAS.EmployerUsers.Support.Web.Controllers
{
    public abstract class SupportController : Controller
    {
        protected readonly IMenuService MenuService;
        protected readonly IMenuTemplateTransformer MenuTemplateTransformer;
        protected static readonly MenuRoot EmptyMenu = new MenuRoot() { MenuItems = new List<MenuItem>(), Perspective = SupportMenuPerspectives.None };
        protected MenuRoot RootMenu = EmptyMenu;
        protected Dictionary<string, string> MenuTransformationIdentifiers = new Dictionary<string, string>();
        protected SupportMenuPerspectives MenuPerspective;

        protected SupportController(IMenuService menuService, IMenuTemplateTransformer menuTemplateTransformer)
        {
            MenuService = menuService;
            MenuTemplateTransformer = menuTemplateTransformer;
          
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext); // do whatever and await the MenuPerspective and Identifiers being set within whatever...

            RootMenu = MenuService.GetMenu(MenuPerspective).Result.SingleOrDefault() ?? EmptyMenu;


            // set the menu into the viewbag for the layour to pick up
            ViewBag.Menu = MenuTemplateTransformer.TransformMenuTemplates(RootMenu.MenuItems,  MenuTransformationIdentifiers);
            
        }
    }
}