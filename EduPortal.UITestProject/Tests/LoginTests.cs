using EduPortal.UITestProject.Pages;
using EduPortal.UITestProject.Tests;
using FluentAssertions;
using OpenQA.Selenium.Support.UI;

namespace EduPortal.UITestProject;

public class LoginTests : BaseTest
{
    [Fact]
    public void EmptyLogin_ShowValidationErrors()
    {
//Arrange
        var loginPage = new LoginPage(Driver, _baseUrl);
        loginPage.Navigate();
//Act
        loginPage.SubmitEmptyForm();
//Assert
        loginPage.GetUserNameErrorMessage().Should().NotBeNullOrEmpty();
        loginPage.GetPasswordErrorMessage().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void EmptyUserNameLogin_ShowValidationError()
    {
        var loginPage = new LoginPage(Driver, _baseUrl);
        loginPage.Navigate();

        loginPage.EnterPassword("somepassword");
        loginPage.ClickLogin();

        loginPage.GetUserNameErrorMessage().Should().NotBeNullOrEmpty();
    }

    [Fact]

    public void EmptyPasswordLogin_ShowValidationError()
    {
        var loginPage = new LoginPage(Driver, _baseUrl);
        loginPage.Navigate();

        loginPage.EnterUsername("someusername");
        loginPage.ClickLogin();

        loginPage.GetPasswordErrorMessage().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void InvalidUserLogin_ShowLoginErrors()
    {
        var loginPage = new LoginPage(Driver, _baseUrl);
        loginPage.Navigate();

        loginPage.EnterUsername("someusername");
        loginPage.EnterPassword("somepassword");
        loginPage.ClickLogin();

        loginPage.GetErrorMessage().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void InvalidPasswordLogin_ShowLoginErrors()
    {
        var loginPage = new LoginPage(Driver, _baseUrl);
        loginPage.Navigate();
        loginPage.EnterUsername("bben");
        loginPage.EnterPassword("invalidpassword");
        loginPage.ClickLogin();
        loginPage.GetErrorMessage().Should().NotBeNullOrEmpty();
    }

    public void InvalidUsernameLogin_ShowLoginErrors()
    {
        var loginPage = new LoginPage(Driver, _baseUrl);
        loginPage.Navigate();
        loginPage.EnterUsername("invalidusername");
        loginPage.EnterPassword("B18n81e1881");
        loginPage.ClickLogin();

        loginPage.GetErrorMessage().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ValidLogin_ShouldRedirectToDashboard()
    {
        var loginPage = new LoginPage(Driver, _baseUrl);
        loginPage.Navigate();

        loginPage.EnterUsername("bben");
        loginPage.EnterPassword("B18n81e1881");
        loginPage.ClickLogin();

        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
        wait.Until(d => d.Url.Contains("/Dashboard"));
        Driver.Url.Should().Contain("/Dashboard");
    }

    [Fact]
    public void ClickOnRegister_ShouldRedirectToRegister()
    {
        var loginPage = new LoginPage(Driver, _baseUrl);
        loginPage.Navigate();

        loginPage.ClickRegister();

        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
        wait.Until(d => d.Url.Contains("/Register"));
        Driver.Url.Should().Contain("/Register");
    }
}
