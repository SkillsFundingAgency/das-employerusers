using OpenQA.Selenium;

namespace SFA.DAS.EmployerUsers.EndToEndTests.EmployerUsersSite
{
    public class LandingPage : PageObject
    {
        public LandingPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public SignInPage ClickSignInButton()
        {
            GetElement("a.button").Click();
            return new SignInPage(WebDriver);
        }
    }
}
