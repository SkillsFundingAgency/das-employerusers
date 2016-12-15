using OpenQA.Selenium;

namespace SFA.DAS.EmployerUsers.EndToEndTests.EmployerUsersSite
{
    public class SignInPage : PageObject
    {
        public SignInPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public string EmailAddress
        {
            get { return GetElement("#email-address").GetValue(); }
            set { GetElement("#email-address").SendKeys(value); }
        }
        public string Password
        {
            get { return GetElement("#password").GetValue(); }
            set { GetElement("#password").SendKeys(value); }
        }

        public RegistrationPage SelectCreateAnAccount()
        {
            GetElement("a[href^=\"/account/register\"]").Click();
            return new RegistrationPage(WebDriver);
        }
        public void ClickSignIn()
        {
            GetElement("button[type=submit]").Click();
        }
    }
}