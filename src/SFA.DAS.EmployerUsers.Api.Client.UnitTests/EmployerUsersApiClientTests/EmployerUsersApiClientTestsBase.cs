using Moq;
using NUnit.Framework;

namespace SFA.DAS.EmployerUsers.Api.Client.UnitTests.EmployerUsersApiClientTests
{
    public abstract class EmployerUsersApiClientTestsBase
    {
        protected EmployerUsersApiClient Client;
        protected Mock<ISecureHttpClient> HttpClient;
        protected EmployerUsersApiConfiguration Configuration;

        [SetUp]
        public void Arrange()
        {
            Configuration = new EmployerUsersApiConfiguration { ApiBaseUrl = "http://some-url/" };
            HttpClient = new Mock<ISecureHttpClient>();
            Client = new EmployerUsersApiClient(Configuration, HttpClient.Object);
        }
    }
}
