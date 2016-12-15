using OpenQA.Selenium;

namespace SFA.DAS.EmployerUsers.EndToEndTests.EmployerUsersSite
{
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