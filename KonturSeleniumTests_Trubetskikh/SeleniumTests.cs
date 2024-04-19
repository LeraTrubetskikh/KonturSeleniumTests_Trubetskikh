using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace KonturSeleniumTests_Trubetskikh;

public class SeleniumTest
{
    private string _login;
    private string _password;
    private ChromeOptions _options;
    private ChromeDriver _driver;
    
    public SeleniumTest()
    {
        GetLoginAndPassword();
        _options = new ChromeOptions();
        _options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
    }
    
    private void GetLoginAndPassword()
    {
        var sr = new StreamReader(@"..\..\..\authorization.txt");
        _login = sr.ReadLine() ?? throw new InvalidOperationException();
        _password = sr.ReadLine() ?? throw new InvalidOperationException();
        sr.Close();
    }
    
    private void Authorization()
    {
        // Перейти по ссылке https://staff-testing.testkontur.ru
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        
        // Ввести логин и пароль
        var login = _driver.FindElement(By.Id("Username"));
        login.SendKeys(_login);
        var password = _driver.FindElement(By.Name("Password"));
        password.SendKeys(_password);
        
        // Нажать на кнопку "Войти"
        var enter = _driver.FindElement(By.Name("button"));
        enter.Click();
        
        // Подождать пока страница загрузится
        _driver.FindElement(By.CssSelector("[data-tid='Title']"));
    }

    [SetUp]
    public void SetUp()
    {
        _driver = new ChromeDriver(_options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }
    
    [Test]
    public void AuthorizationTest()
    {
        Authorization();

        // Проверить что находимся на нужной странице
        var currentUrl = _driver.Url;
        currentUrl.Should().Be("https://staff-testing.testkontur.ru/news");
    }

    [TearDown]
    public void TearDown()
    {
        _driver.Close();
        _driver.Quit();
    }
}