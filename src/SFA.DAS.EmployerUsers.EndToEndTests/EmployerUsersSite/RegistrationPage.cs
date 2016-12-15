using OpenQA.Selenium;

namespace SFA.DAS.EmployerUsers.EndToEndTests.EmployerUsersSite
{
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
}