using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCloudStorage.Test.RegistrationUser
{
    internal class RegistrationUser
    {
        public string Email = "bobreczovm@inbox.ru";
        public string password = "1";
        public string Name = "Maksin";

        public IWebElement Filters;

        public string SelectorName = "div.page:nth-child(1) main:nth-child(2) article.content.px-4 div:nth-child(2) > input:nth-child(2)";

        public string SelectorPassword = "div.page:nth-child(1) main:nth-child(2) article.content.px-4 div:nth-child(3) > input:nth-child(2)";

        public string SelectorMail = "div.page:nth-child(1) main:nth-child(2) article.content.px-4 div:nth-child(4) > input:nth-child(2)";
        public string SelectorCreateUser = "body:nth-child(2) div.page:nth-child(1) main:nth-child(2) article.content.px-4 > button.btn.btn-primary:nth-child(5)";

        public WebDriverWait wait { get; set; }

        public void Parameth(WebDriverWait driver)
        {

            wait = driver;
        }

        [Test]

        public void Rigistration()
        {
            try
            {

                Filters = wait.Until(d => d.FindElement(By.CssSelector(SelectorName)));
                Filters.SendKeys(Name);
                Filters = wait.Until(d => d.FindElement(By.CssSelector(SelectorPassword)));
                Filters.SendKeys(password);
                Filters = wait.Until(d => d.FindElement(By.CssSelector(SelectorMail)));
               Filters.SendKeys(Email);
               Filters = wait.Until(d => d.FindElement(By.CssSelector(SelectorCreateUser)));
               Filters.Click();
            }
            catch (Exception ex)
            {

            }
        }

    }
}
