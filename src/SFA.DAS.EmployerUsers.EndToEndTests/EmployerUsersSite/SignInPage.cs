using OpenQA.Selenium;

namespace SFA.DAS.EmployerUsers.EndToEndTests.EmployerUsersSite
{
    public class SignInPage : PageObject
    {
        public SignInPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public RegistrationPage SelectCreateAnAccount()
        {
            GetElement("a[href^=\"/account/register\"]").Click();
            return new RegistrationPage(WebDriver);
        }
    }
}