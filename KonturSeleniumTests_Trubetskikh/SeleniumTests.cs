﻿using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace KonturSeleniumTests_Trubetskikh;

public class SeleniumTest
{
    private readonly List<string> _loginAndPassword;
    private ChromeOptions _options;
    private ChromeDriver _driver;
    
    public SeleniumTest()
    {
        _loginAndPassword = GetLoginAndPassword();
        _options = new ChromeOptions();
        _options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
    }
    
    private List<string> GetLoginAndPassword()
    {
        var sr = new StreamReader(@"..\..\..\authorization.txt");
        var login = sr.ReadLine();
        var password = sr.ReadLine();
        sr.Close();
        return new List<string> {login, password};
    }

    [SetUp]
    public void SetUp()
    {
        _driver = new ChromeDriver(_options);
    }
    
    [Test]
    public void Authorization()
    {
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        Thread.Sleep(5000); // исправить

        var login = _driver.FindElement(By.Id("Username"));
        login.SendKeys(_loginAndPassword[0]);
        
        var password = _driver.FindElement(By.Name("Password"));
        password.SendKeys(_loginAndPassword[1]);

        var enter = _driver.FindElement(By.Name("button"));
        enter.Click();
        
        Thread.Sleep(5000); // исправить

        var currentUrl = _driver.Url;
        Assert.That(currentUrl == "https://staff-testing.testkontur.ru/news");
    }

    [TearDown]
    public void TearDown()
    {
        _driver.Quit();
    }
}