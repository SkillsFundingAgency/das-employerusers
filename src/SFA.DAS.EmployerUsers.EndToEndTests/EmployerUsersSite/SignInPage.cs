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

    public class RegistrationPage : PageObject
    {
        public RegistrationPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public string FirstName
        {
            get { return GetElement("#FirstName").GetValue(); }
            set { GetElement("#FirstName").SendKeys(value); }
        }
        public string LastName
        {
            get { return GetElement("#LastName").GetValue(); }
            set { GetElement("#LastName").SendKeys(value); }
        }
        public string Email
        {
            get { return GetElement("#Email").GetValue(); }
            set { GetElement("#Email").SendKeys(value); }
        }
        public string Password
        {
            get { return GetElement("#Password").GetValue(); }
            set { GetElement("#Password").SendKeys(value); }
        }
        public string ConfirmPassword
        {
            get { return GetElement("#ConfirmPassword").GetValue(); }
            set { GetElement("#ConfirmPassword").SendKeys(value); }
        }

        public AccountConfirmationPage ClickSetUp()
        {
            GetElement("button[type=submit]").Click();
            return new AccountConfirmationPage(WebDriver);
        }
    }

    public class AccountConfirmationPage : PageObject
    {
        public AccountConfirmationPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public string AccessCode
        {
            get { return GetElement("#AccessCode").GetValue(); }
            set { GetElement("#AccessCode").SendKeys(value); }
        }

        public void ClickContinue()
        {
            GetElement("button[type=submit]").Click();
        }
    }
}