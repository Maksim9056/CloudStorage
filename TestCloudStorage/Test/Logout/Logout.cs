using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCloudStorage.Test.Logout
{
    internal class Logout
    {

        //https://github.com/Maksim9056/CloudStorage.git
        public IWebElement Filters;

        public string SelectorEmail = "div.page:nth-child(1) main:nth-child(2) article.content.px-4 div:nth-child(2) > input:nth-child(2)";
        public string SelectorPassword= "div.page:nth-child(1) main:nth-child(2) article.content.px-4 div:nth-child(3) > input:nth-child(2)";
        public string SelectorLogin = "div.page:nth-child(1) main:nth-child(2) article.content.px-4 div:nth-child(5) > button.button";
        public string SelectorRegistation = "/html/body/div[1]/main/article/button";
        public string Email = "bobreczovm@inbox.ru";
        public string password = "1";
        public WebDriverWait wait { get; set; }

        public void Parameth(WebDriverWait driver)
        {

            wait = driver;
        }
        [Test]

        public void Login()
        {
            try
            {


                var Filters = wait.Until(d => d.FindElement(By.CssSelector(SelectorEmail)));
                Filters.SendKeys(Email);
                Filters = wait.Until(d => d.FindElement(By.CssSelector(SelectorPassword)));
                Filters.SendKeys(password);
                Filters = wait.Until(d => d.FindElement(By.CssSelector(SelectorLogin)));
                Filters.Click();
            }catch (Exception ex)
            {

            }
        }
        
        [Test]
        public void Rigistration()
        {
            try
            {

                var Filters = wait.Until(d => d.FindElement(By.CssSelector("body > div.page > main > article > div:nth-child(6) > button")));
                Filters.Click();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
