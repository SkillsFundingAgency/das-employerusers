using NUnit.Framework;
using OpenQA.Selenium;

namespace SFA.DAS.EmployerUsers.EndToEndTests.Tests
{
    public class LoginTests : TestBase
    {
        private Contexts.TestContext _context;

        [SetUp]
        public void Arrange()
        {
            CreateWebDriver();

            _context = new Contexts.TestContext();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyWebDriver();
        }

        [Test]
        public void HappyPath()
        {
            Data.CreateUser(_context.EmailAddress, _context.Password, _context.FirstName, _context.LastName);

            NavigateToSigninPage();

            SignIn(_context.EmailAddress, _context.Password);

            var logoutButton = WebDriver.FindElement(By.CssSelector("a[href^=\"/account/logout\"]"));
            Assert.IsNotNull(logoutButton);
        }

    }
}
