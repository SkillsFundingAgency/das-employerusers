using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SFA.DAS.EmployerUsers.EndToEndTests.EmployerUsersSite;

namespace SFA.DAS.EmployerUsers.EndToEndTests
{
    public abstract class TestBase
    {
        protected TestBase()
        {
            Settings = new TestSettings();
            Data = new DataHelper(Settings);
        }

        protected TestSettings Settings { get; }
        protected DataHelper Data { get; }

        protected IWebDriver WebDriver { get; private set; }
        protected SignInPage SignInPage { get; private set; }
        protected RegistrationPage RegistationPage { get; private set; }
        protected AccountConfirmationPage AccountConfirmationPage { get; private set; }

        protected void CreateWebDriver()
        {
            WebDriver = new ChromeDriver();
        }
        protected void DestroyWebDriver()
        {

            WebDriver.Close();
            WebDriver.Quit();
            WebDriver.Dispose();
        }

        protected void NavigateToSigninPage()
        {
            WebDriver.Navigate().GoToUrl(Settings.EmployerUsersUrl);
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
        protected void ConfirmAccount(string accessCode)
        {
            if (AccountConfirmationPage == null)
            {
                throw new Exception("Not on registration page");
            }

            AccountConfirmationPage.AccessCode = accessCode;
            AccountConfirmationPage.ClickContinue();
            AccountConfirmationPage = null;
        }

        protected void SignIn(string emailAddress, string password)
        {
            if (SignInPage == null)
            {
                throw new Exception("Not on signin page");
            }

            SignInPage.EmailAddress = emailAddress;
            SignInPage.Password = password;
            SignInPage.ClickSignIn();
            SignInPage = null;
        }

    }
}
