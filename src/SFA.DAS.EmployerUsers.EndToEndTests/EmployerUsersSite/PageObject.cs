using OpenQA.Selenium;

namespace SFA.DAS.EmployerUsers.EndToEndTests.EmployerUsersSite
{
    public abstract class PageObject
    {
        protected PageObject(IWebDriver webDriver)
        {
            WebDriver = webDriver;
        }

        protected IWebDriver WebDriver { get; }

        protected IWebElement GetElement(string cssSelector)
        {
            return GetElement(By.CssSelector(cssSelector));
        }
        protected IWebElement GetElement(By selector)
        {
            var elem = WebDriver.FindElement(selector);
            if (elem == null)
            {
                throw new NoSuchElementException($"Cannot find email address input (Searching by selector '{selector}')");
            }
            return elem;
        }
    }
}
