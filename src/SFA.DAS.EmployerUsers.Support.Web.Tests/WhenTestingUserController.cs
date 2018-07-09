using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerUsers.Support.Core.Domain.Model;
using SFA.DAS.EmployerUsers.Support.Infrastructure;
using SFA.DAS.EmployerUsers.Support.Web.Controllers;
using SFA.DAS.Support.Shared.Discovery;
using SFA.DAS.Support.Shared.Navigation;

namespace SFA.DAS.EmployerUsers.Support.Web.Tests
{
    public class WhenTestingUserController
    {
        protected readonly string _id = "123";
        protected Mock<IEmployerUserRepository> _mockEmployerUserRepository;
        protected Mock<IServiceConfiguration> _mockServiceConfiguration;
        protected UserController _unit;
        protected EmployerUser _user;
        protected Mock<IMenuService> _mockMenuService;
        protected Mock<IMenuTemplateTransformer> _mockMenuTemplateTransformer;
        [SetUp]
        public void Setup()
        {
            _mockEmployerUserRepository = new Mock<IEmployerUserRepository>();
            _mockServiceConfiguration = new Mock<IServiceConfiguration>();

            _mockMenuService = new Mock<IMenuService>();
            _mockMenuTemplateTransformer = new Mock<IMenuTemplateTransformer>();

            _unit = new UserController(_mockEmployerUserRepository.Object, 
                _mockServiceConfiguration.Object, 
                _mockMenuService.Object, 
                _mockMenuTemplateTransformer.Object);
        }
    }
}