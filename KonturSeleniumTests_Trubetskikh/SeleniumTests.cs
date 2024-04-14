using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace KonturSeleniumTests_Trubetskikh;

public class SeleniumTest
{
    [Test]
    public void Authorization()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        var driver = new ChromeDriver(options);
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        Thread.Sleep(5000); // исправить

        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("");
        
        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("");

        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
        
        Thread.Sleep(5000); // исправить

        var currentUrl = driver.Url;
        Assert.That(currentUrl == "https://staff-testing.testkontur.ru/news");
        
        driver.Quit();
    }
}