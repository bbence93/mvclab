using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduPortal.UITestProject.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl;
        private const string? Pagepath = null;

        public LoginPage(IWebDriver driver, string baseUrl)
        {
            _driver = driver;
            _baseUrl = baseUrl;
        }

        private IWebElement _usernameField => _driver.FindElement(By.Id("UserName"));
        private IWebElement _passwordField => _driver.FindElement(By.Id("Password"));
        private IWebElement _rememberMeCheckbox => _driver.FindElement(By.Id("RememberMe"));
        private IWebElement LoginButton => _driver.FindElement(By.CssSelector("button[type='submit']"));
        private IWebElement RegisterButton => _driver.FindElement(By.Id("registerBtn"));
        private IWebElement UserNameErrorElement => _driver.FindElement(By.CssSelector("span[data-valmsg-for='UserName']"));
        private IWebElement PasswordErrorElement => _driver.FindElement(By.CssSelector("span[data-valmsg-for='Password']"));
        private IWebElement SuccessAlert => _driver.FindElement(By.Id("successAlert"));
        private IWebElement FailureAlert => _driver.FindElement(By.Id("errorAlert"));

        public void Navigate()
        {
            _driver.Navigate().GoToUrl(_baseUrl);
        }

        public void EnterUsername(string username)
        {
            _usernameField.Clear();
            _usernameField.SendKeys(username);
        }

        public void EnterPassword(string password)
        {
            _passwordField.Clear();
            _passwordField.SendKeys(password);
        }

        public void ClickLogin()
        {
            LoginButton.Click();
        }

        public void ClickRegister()
        {
            RegisterButton.Click();
        }

        public void PerformLogin(string username, string password, bool rememberMe = false)
        {
            EnterUsername(username);
            EnterPassword(password);
            if (rememberMe)
            {
                _rememberMeCheckbox.Click();
            }
            ClickLogin();
        }

        public string GetUserNameErrorMessage()
        {
            return UserNameErrorElement.Text;
        }

        public string GetPasswordErrorMessage()
        {
            return PasswordErrorElement.Text;
        }

        public void SubmitEmptyForm()
        {
            LoginButton.Click();
        }

        public string GetSuccessMessage()
        {
            try
            {
                // Visszaadja pl: "Sikeres bejelentkezés!"
                return SuccessAlert.Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty; // Vagy null, ha nincs üzenet
            }
        }

        public string GetErrorMessage()
        {
            try
            {
                // Visszaadja pl: "Érvénytelen felhasználónév vagy jelszó."
                return FailureAlert.Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty; // Vagy null, ha nincs üzenet
            }
        }
    }
}
