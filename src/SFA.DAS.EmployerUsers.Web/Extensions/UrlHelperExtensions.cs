using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmployerAccountsAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerUsersConfiguration>();
            var baseUrl = configuration.EmployerAccountsBaseUrl;

            return Action(baseUrl, path);
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;
            return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
        }
    }
}