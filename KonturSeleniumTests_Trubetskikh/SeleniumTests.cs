using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace KonturSeleniumTests_Trubetskikh;

public class SeleniumTest
{
    private readonly string _login;
    private readonly string _password;
    private readonly ChromeOptions _options;
    private ChromeDriver _driver;
    private string _createdCommunity;
    
    public SeleniumTest()
    {
        _options = new ChromeOptions();
        _options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        _createdCommunity = "";
        
        var sr = new StreamReader(@"..\..\..\authorization.txt");
        _login = sr.ReadLine() ?? throw new InvalidOperationException("В файле authorization.txt не хватает логина и пароля");
        _password = sr.ReadLine() ?? throw new InvalidOperationException("В файле authorization.txt не хватает логина или пароля");
        sr.Close();
    }

    [SetUp]
    public void SetUp()
    {
        _driver = new ChromeDriver(_options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        Authorization();
    }
    
    [Test]
    public void AuthorizationTest()
    {
        // Проверить что находимся на нужной странице
        var currentUrl = _driver.Url;
        currentUrl.Should().Be("https://staff-testing.testkontur.ru/news");
    }
    
    private void Authorization()
    {
        // 1.Перейти по ссылке https://staff-testing.testkontur.ru
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        
        // 2.Ввести логин и пароль
        var login = _driver.FindElement(By.Id("Username"));
        login.SendKeys(_login);
        var password = _driver.FindElement(By.Name("Password"));
        password.SendKeys(_password);
        
        // 3.Нажать на кнопку "Войти"
        var enter = _driver.FindElement(By.Name("button"));
        enter.Click();
        
        // 4.Подождать пока страница загрузится
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Title']")));
    }
    
    [Test]
    public void LogoutTest()
    {
        // 1.Нажать на иконку профиля
        var profile = _driver.FindElement(By.CssSelector("[data-tid='Avatar']"));
        profile.Click();
        
        // 2.Нажать на кнопку выйти
        var logout = _driver.FindElement(By.CssSelector("[data-tid='Logout']"));
        logout.Click();
        
        // 3.Проверить что вышли
        var element = _driver.FindElement(By.ClassName("login-page"));
        element.Should().NotBeNull();
    }
    
    [Test]
    public void SearchTest()
    {
        // 1.Нажать на строку поиска
        var searchBar = _driver.FindElement(By.CssSelector("[data-tid='SearchBar']"));
        searchBar.Click();
        searchBar = searchBar.FindElement(By.TagName("input"));
        
        // 2.Ввести что-то
        searchBar.SendKeys("a");
        
        // 3.Проверить что появляется список ответов
        var items = _driver.FindElements(By.CssSelector("[data-tid='ComboBoxMenu__item']"));
        items.Count.Should().BePositive();
    }
    
    [Test]
    public void OpenProfileAvatarTest()
    {
        // 1.Перейти по ссылке на профиль
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/profile/6adf50eb-6273-4b16-ba77-ff911cea4cfa");
        
        // 2.Нажать на аватарку
        var avatar = _driver.FindElement(By.CssSelector("[data-tid='PageHeader']"))
            .FindElement(By.CssSelector("[data-tid='Avatar']"));
        avatar.Click();      
        
        // 3.Проверить что картинка открылась
        var picture = _driver
            .FindElement(By.CssSelector("[src='/api/v1/images/9b747a0c-8b26-4315-90a7-09731250eebf/800x800'"));
        picture.Should().NotBeNull();
    }

    [Test]
    public void CreateCommunityTest()
    {
        // 1.Сгенерировать название сообщества
        var guid = Guid.NewGuid().ToString();
        
        // 2.Создать сообщество
        var url = CreateCommunity(guid);
        
        // 3.Открыть сообщество
        _driver.Navigate().GoToUrl(url);
        
        // 4.Сверить название созданного сообщества и сгенерированное название
        var currentName = _driver.FindElement(By.CssSelector("[data-tid='Title']")).Text;
        currentName.Should().Be(guid);
    }

    private string CreateCommunity(string name)
    {
        // 1.Перейти по ссылке https://staff-testing.testkontur.ru/communities
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");

        // 2.Нажать на кнопку создать
        var createCommunity = _driver.FindElement(By.CssSelector("[data-tid='PageHeader']"))
            .FindElement(By.TagName("button"));
        createCommunity.Click();

        // 3.Ввести название
        var fieldName = _driver.FindElement(By.CssSelector("[data-tid='Name']"));
        fieldName.Click();
        fieldName.SendKeys(name);
        
        // 4.Нажать "Создать"
        var createButton = _driver.FindElement(By.CssSelector("[data-tid='CreateButton']"));
        createButton.Click();
        
        // 5.Подождать пока страница загрузится и вернуть ссылку
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='SettingsTabWrapper']")));
        var url = _driver.Url.Substring(0, _driver.Url.Length - 9);
        _createdCommunity = url;
        
        return url;
    }
    
    [Test]
    public void DeleteCommunityTest()
    {
        // 1.Создать сообщество
        var guid = Guid.NewGuid().ToString();
        var url = CreateCommunity(guid);
        
        // 2.Удалить сообщество
        DeleteCommunity(url);
        
        // 3.Проверить что сообщество удалено
        _driver.Navigate().GoToUrl(url);
        var message = _driver.FindElement(By.CssSelector("[data-tid='ValidationMessage']"));
        message.Should().NotBeNull();
    }

    private void DeleteCommunity(string url)
    {
        // 1.Перейти в настройки сообщества
        _driver.Navigate().GoToUrl($"{url}/settings");
        
        // 2.Нажать кнопку удалить
        var deleteButton = _driver.FindElement(By.CssSelector("[data-tid='DeleteButton']"));
        deleteButton.Click();

        // 3.Подтвердить удаление
        var delete = _driver.FindElement(By.CssSelector("[data-tid='ModalPageFooter']"))
            .FindElement(By.CssSelector("[data-tid='DeleteButton']"));
        delete.Click();
        _createdCommunity = "";
    }

    [TearDown]
    public void TearDown()
    {
        if (_createdCommunity != "")
        {
            DeleteCommunity(_createdCommunity);
            _createdCommunity = "";
        }

        _driver.Close();
        _driver.Quit();
    }
}