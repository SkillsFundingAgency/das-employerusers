using System.Collections.Generic;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using MediatR;
using StructureMap;

namespace SFA.DAS.EmployerUsers.Web.Authentication
{
    public static class IdentityServerServiceFactoryExtensions
    {
        public static IdentityServerServiceFactory RegisterDasServices(this IdentityServerServiceFactory factory, IContainer container)
        {
            factory.Register(new Registration<SingleInstanceFactory>((dr) => container.GetInstance<SingleInstanceFactory>()));
            factory.Register(new Registration<MultiInstanceFactory>((dr) => container.GetInstance<MultiInstanceFactory>()));
            factory.Register(new Registration<IMediator>((dr) => container.GetInstance<IMediator>()));

            return factory;
        }

        public static IdentityServerServiceFactory UseDasUserService(this IdentityServerServiceFactory factory)
        {
            factory.Register(new Registration<List<UserService>>());
            factory.UserService = new Registration<IUserService, UserService>();
            return factory;
        }
    }
}