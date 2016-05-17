using System.Collections.Generic;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;

namespace SFA.DAS.EmployerUsers.Web.Authentication
{
    public static class IdentityServerServiceFactoryExtensions
    {
        public static IdentityServerServiceFactory UseDasUserService(this IdentityServerServiceFactory factory)
        {
            factory.Register<List<UserService>>(new Registration<List<UserService>>());
            factory.UserService = (Registration<IUserService>)new Registration<IUserService, UserService>((string)null);
            return factory;
        }
    }
}