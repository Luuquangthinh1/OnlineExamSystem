using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
namespace OnlineExamSystem.Tests;

public class RegisterTests : IDisposable
{
    private readonly IWebDriver _driver;

    public RegisterTests()
    {
        var options = new ChromeOptions();

        //options.AddArgument("--headless=new");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AcceptInsecureCertificates = true;

        _driver = new ChromeDriver(options);
    }

    [Fact]
    public void Register_Student_Success()
    {
        Console.WriteLine("1. Start test");

        _driver.Navigate()
               .GoToUrl("https://localhost:7195/Auth/Register");

        Console.WriteLine("2. Opened Register page");

        Thread.Sleep(5000);

        string username = "student01";

        _driver.FindElement(By.Name("username"))
               .SendKeys(username);

        Console.WriteLine("3. Username entered");

        _driver.FindElement(By.Name("password"))
               .SendKeys("123456");

        Console.WriteLine("4. Password entered");

        var role = new SelectElement(
            _driver.FindElement(By.Name("role")));

        role.SelectByText("Student");

        Console.WriteLine("5. Role selected");

        _driver.FindElement(
            By.CssSelector("button[type='submit']"))
            .Click();

        Console.WriteLine("6. Submit clicked");

        Thread.Sleep(5000);

        Console.WriteLine("7. Current URL = " + _driver.Url);
        Assert.StartsWith(
    "https://localhost:7195",
    _driver.Url);

    }
    public void Dispose()
    {
        _driver.Quit();
    }
}