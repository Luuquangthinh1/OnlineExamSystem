using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace OnlineExamSystem.Tests;

public class RegisterTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _baseUrl;

    public RegisterTests()
    {
        _factory = new CustomWebApplicationFactory();

        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _baseUrl = client.BaseAddress!.ToString().TrimEnd('/');

        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--window-size=1920,1080");
        options.AcceptInsecureCertificates = true;

        _driver = new ChromeDriver(options);
    }

    [Fact]
    public void Register_Student_Success()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/Auth/Register");
        Thread.Sleep(2000);

        _driver.FindElement(By.Name("username")).SendKeys("student01");
        _driver.FindElement(By.Name("password")).SendKeys("123456");

        var role = new SelectElement(_driver.FindElement(By.Name("role")));
        role.SelectByText("Student");

        _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        Thread.Sleep(3000);

        Assert.StartsWith(_baseUrl, _driver.Url);
    }

    public void Dispose()
    {
        _driver.Quit();
        _factory.Dispose();
    }
}
