using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.ViewModels;
using NLog;
using SFA.DAS.EmployerUsers.Web.Controllers;

namespace SFA.DAS.EmployerUsers.Web.Authentication
{
    public class CustomIdsViewService : DefaultViewService
    {
        private readonly ILogger _logger;
        private readonly IControllerFactory _controllerFactory;

        public CustomIdsViewService(DefaultViewServiceOptions config, IViewLoader viewLoader)
            : this(config, viewLoader, LogManager.GetCurrentClassLogger(), ControllerBuilder.Current.GetControllerFactory())
        {
        }
        public CustomIdsViewService(DefaultViewServiceOptions config, IViewLoader viewLoader, ILogger logger, IControllerFactory controllerFactory)
            : base(config, viewLoader)
        {
            _logger = logger;
            _controllerFactory = controllerFactory;
        }

        public override Task<Stream> Error(ErrorViewModel model)
        {
            _logger.Error($"IDS Error view rendering for error '{model.ErrorMessage}'");

            var controller = GetController<ErrorController>();
            var result = RenderGeneralErrorAction(controller);

            return Task.FromResult(result);
        }
        public override Task<Stream> AuthorizeResponse(AuthorizeResponseViewModel model)
        {
            var controller = GetController<LoginController>();
            var result = RenderAuthorizeResponseAction(controller, model);

            return Task.FromResult(result);
        }
        public override Task<Stream> LoggedOut(LoggedOutViewModel model, SignOutMessage message)
        {
            var controller = GetController<LoginController>();
            var result = RenderLogoutAction(controller, model, message);

            return Task.FromResult(result);
        }



        private HttpContextWrapper DuplicateAndWrapHttpContext(HttpContext current)
        {
            var request = new HttpRequest(null, current.Request.Url.ToString(), current.Request.Url.Query);
            var response = new HttpResponse(new StringWriter());
            var context = new HttpContext(request, response);

            // Setup cookies
            foreach (var key in current.Request.Cookies.AllKeys)
            {
                var cookie = current.Request.Cookies[key];
                if (cookie != null)
                {
                    request.Cookies.Set(cookie);
                }
            }

            // Needed for "owin.environment"
            foreach (var key in current.Items.Keys)
            {
                context.Items[key] = current.Items[key];
            }

            return new HttpContextWrapper(context);
        }
        private T GetController<T>(string controllerName = null)
            where T : Controller
        {
            if (string.IsNullOrEmpty(controllerName))
            {
                var typeName = typeof(T).Name;
                if (!typeName.EndsWith("Controller"))
                {
                    throw new System.Exception($"Cannot infer controller name of type {typeName}");
                }

                controllerName = typeName.Substring(0, typeName.Length - 10);
            }

            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);

            var httpContext = DuplicateAndWrapHttpContext(HttpContext.Current);

            var requestContext = new RequestContext(httpContext, routeData);
            var controller = _controllerFactory.CreateController(requestContext, controllerName) as T;
            if (controller != null)
            {
                controller.ControllerContext = new ControllerContext(httpContext, routeData, controller);
            }

            return controller;
        }


        private Stream RenderAuthorizeResponseAction(LoginController controller, AuthorizeResponseViewModel model)
        {
            using (var outputWriter = new StringWriter())
            {
                controller.ControllerContext.RequestContext.HttpContext.Response.Output = outputWriter;
                controller.ControllerContext.RouteData.Values.Add("action", "AuthorizeResponse");
                controller.ControllerContext.RouteData.Values["model"] = model;

                var viewResult = controller.AuthorizeResponse(model);
                viewResult.ExecuteResult(controller.ControllerContext);

                outputWriter.Flush();
                var content = outputWriter.ToString();

                return new MemoryStream(Encoding.UTF8.GetBytes(content));
            }
        }
        private Stream RenderLogoutAction(LoginController controller, LoggedOutViewModel model, SignOutMessage message)
        {
            using (var outputWriter = new StringWriter())
            {
                controller.ControllerContext.RequestContext.HttpContext.Response.Output = outputWriter;
                controller.ControllerContext.RouteData.Values.Add("action", "Logout");
                controller.ControllerContext.RouteData.Values["model"] = model;

                var viewResult = controller.Logout(model, message);
                viewResult.ExecuteResult(controller.ControllerContext);

                outputWriter.Flush();
                var content = outputWriter.ToString();

                return new MemoryStream(Encoding.UTF8.GetBytes(content));
            }
        }


        private Stream RenderGeneralErrorAction(ErrorController controller)
        {
            using (var outputWriter = new StringWriter())
            {
                controller.ControllerContext.RequestContext.HttpContext.Response.Output = outputWriter;
                controller.ControllerContext.RouteData.Values.Add("action", "General");

                var viewResult = controller.General();
                viewResult.ExecuteResult(controller.ControllerContext);

                outputWriter.Flush();
                var content = outputWriter.ToString();

                return new MemoryStream(Encoding.UTF8.GetBytes(content));
            }
        }

    }
}