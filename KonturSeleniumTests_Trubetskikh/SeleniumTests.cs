using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace KonturSeleniumTests_Trubetskikh;

public class SeleniumTest
{
    private readonly List<string> _loginAndPassword;
    
    public SeleniumTest()
    {
        _loginAndPassword = GetLoginAndPassword();
    }
    
    private List<string> GetLoginAndPassword()
    {
        var sr = new StreamReader(@"..\..\..\authorization.txt");
        var login = sr.ReadLine();
        var password = sr.ReadLine();
        sr.Close();
        return new List<string> {login, password};
    }
    
    [Test]
    public void Authorization()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        var driver = new ChromeDriver(options);
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        Thread.Sleep(5000); // исправить

        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys(_loginAndPassword[0]);
        
        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys(_loginAndPassword[1]);

        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
        
        Thread.Sleep(5000); // исправить

        var currentUrl = driver.Url;
        Assert.That(currentUrl == "https://staff-testing.testkontur.ru/news");
        
        driver.Quit();
    }
}