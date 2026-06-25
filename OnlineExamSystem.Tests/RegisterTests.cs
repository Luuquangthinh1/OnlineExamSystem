using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace OnlineExamSystem.Tests;

public class RegisterTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly Process _appProcess;
    private readonly string _baseUrl = "https://127.0.0.1:7195";

    public RegisterTests()
    {
        var appProjectPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "OnlineExamSystem"));

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --no-build --urls https://127.0.0.1:7195",
            WorkingDirectory = appProjectPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _appProcess = Process.Start(psi) ?? throw new InvalidOperationException("Cannot start app process.");

        WaitForServerReady(_baseUrl, TimeSpan.FromSeconds(60));

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

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        wait.Until(d => d.FindElement(By.Name("username")));

        _driver.FindElement(By.Name("username")).SendKeys("student01");
        _driver.FindElement(By.Name("password")).SendKeys("123456");

        var role = new SelectElement(_driver.FindElement(By.Name("role")));
        role.SelectByText("Student");

        _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        wait.Until(d => d.Url.StartsWith(_baseUrl, StringComparison.OrdinalIgnoreCase));
        Assert.StartsWith(_baseUrl, _driver.Url);
    }

    private static void WaitForServerReady(string baseUrl, TimeSpan timeout)
    {
        using var http = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        });

        var sw = Stopwatch.StartNew();
        while (sw.Elapsed < timeout)
        {
            try
            {
                var resp = http.GetAsync($"{baseUrl}/Auth/Login").GetAwaiter().GetResult();
                if ((int)resp.StatusCode < 500) return;
            }
            catch
            {
                // app chưa lên xong
            }

            Thread.Sleep(500);
        }

        throw new TimeoutException($"Server not ready at {baseUrl} within {timeout.TotalSeconds}s.");
    }

    public void Dispose()
    {
        try
        {
            _driver.Quit();
            _driver.Dispose();
        }
        catch { }

        try
        {
            if (!_appProcess.HasExited)
            {
                _appProcess.Kill(entireProcessTree: true);
                _appProcess.WaitForExit(5000);
            }
            _appProcess.Dispose();
        }
        catch { }
    }
}
