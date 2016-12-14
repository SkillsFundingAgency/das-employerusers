using NUnit.Framework;
using SFA.DAS.EmployerUsers.EndToEndTests.Contexts;

namespace SFA.DAS.EmployerUsers.EndToEndTests
{
    public class RegistrationTests : TestBase
    {
        private RegistrationContext _context;

        [SetUp]
        public void Arrange()
        {
            CreateWebDriver();

            _context = new RegistrationContext();
        }

        [TearDown]
        public void TearDown()
        {
            WebDriver.Close();
            WebDriver.Quit();
            WebDriver.Dispose();
        }

        [Test]
        public void HappyPath()
        {
            NavigateToSigninPage();

            StartRegistation();

            CreateAccount(_context.FirstName, _context.LastName, _context.EmailAddress, _context.Password, _context.Password);

            _context.AccessCode = Data.GetAccessCodeForUser(_context.EmailAddress);
            ConfirmAccount(_context.AccessCode);

            var isUserActive = Data.IsUserActive(_context.EmailAddress);
            Assert.IsTrue(isUserActive);
        }
    }
}
