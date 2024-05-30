using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace TestCloudStorage
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            var edgeOptions = new EdgeOptions();
            IWebDriver driver = new EdgeDriver(@"C:\msedgedriver.exe", edgeOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            driver.Navigate().GoToUrl("https://localhost:7256/");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            Logout  logout = new Logout();
            logout.Parameth(wait);
            logout.Rigistration();
            RegistrationUser registrationUser = new RegistrationUser();
            registrationUser.Parameth(wait);
            registrationUser.Rigistration();
            logout.Login();

        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}