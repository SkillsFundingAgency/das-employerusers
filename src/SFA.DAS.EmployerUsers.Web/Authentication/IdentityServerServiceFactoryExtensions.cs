using System.Collections.Generic;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using MediatR;

namespace SFA.DAS.EmployerUsers.Web.Authentication
{
    public static class IdentityServerServiceFactoryExtensions
    {
        public static IdentityServerServiceFactory RegisterDasServices(this IdentityServerServiceFactory factory)
        {
            factory.Register(new Registration<IMediator, Mediator>());

            return factory;
        }

        public static IdentityServerServiceFactory UseDasUserService(this IdentityServerServiceFactory factory)
        {
            factory.Register<List<UserService>>(new Registration<List<UserService>>());
            factory.UserService = (Registration<IUserService>)new Registration<IUserService, UserService>((string)null);
            return factory;
        }
    }
}