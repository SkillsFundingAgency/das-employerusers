using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SFA.DAS.EmployerUsers.EndToEndTests.EmployerUsersSite;

namespace SFA.DAS.EmployerUsers.EndToEndTests
{
    public abstract class TestBase
    {
        protected IWebDriver WebDriver { get; private set; }
        protected SignInPage SignInPage { get; private set; }
        protected RegistrationPage RegistationPage { get; private set; }
        protected AccountConfirmationPage AccountConfirmationPage { get; private set; }

        protected void CreateWebDriver()
        {
            WebDriver = new ChromeDriver();
        }
        protected void NavigateToSigninPage()
        {
            WebDriver.Navigate().GoToUrl("https://dev-employer.apprenticeships.sfa.bis.gov.uk/");
            var landingPage =  new LandingPage(WebDriver);

            SignInPage =landingPage.ClickSignInButton();
        }

        protected void StartRegistation()
        {
            if (SignInPage == null)
            {
                throw new Exception("Not on signin page");
            }

            RegistationPage = SignInPage.SelectCreateAnAccount();
            SignInPage = null;
        }
        protected void CreateAccount(string firstName, string lastName, string emailAddress, string password, string confirmPassword)
        {
            if (RegistationPage == null)
            {
                throw new Exception("Not on registration page");
            }

            RegistationPage.FirstName = firstName;
            RegistationPage.LastName = lastName;
            RegistationPage.Email = emailAddress;
            RegistationPage.Password = password;
            RegistationPage.ConfirmPassword = confirmPassword;
            AccountConfirmationPage = RegistationPage.ClickSetUp();
            RegistationPage = null;
        }

    }
}
