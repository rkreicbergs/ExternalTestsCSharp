using System;
using System.Drawing;
using ExternalTests.Common;
using ExternalTests.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Xunit;

#pragma warning disable SA1202

namespace ExternalTests.Ui
{
    public class SeleniumBrowserWrapper : BrowserWrapperBase
    {
        private static readonly TimeSpan CommandTimeOut = TimeSpan.FromSeconds(30);

        private readonly IWebDriver _driver;
       
        public SeleniumBrowserWrapper(Logger logger) : base(logger)
        {
            // TODO: add support for other browsers
            Assert.Equal("Chrome", TestSettings.Get.SeleniumSettings.Browser);
            
            if (TestSettings.Get.SeleniumSettings.UseLocalBrowser)
            {
                _driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), new ChromeOptions(), CommandTimeOut);
            }
            else
            {
                var seleniumGridHubUrl = new Uri(TestSettings.Get.SeleniumSettings.GridUrl);
                Logger.Info("Selenium Grid URL: " + seleniumGridHubUrl);
                _driver = new RemoteWebDriver(seleniumGridHubUrl, new ChromeOptions().ToCapabilities(), CommandTimeOut);
            }

            try
            {
                _driver.Manage().Window.Size=new Size(1936, 1056);
                _driver.Manage().Window.Position = new Point(-8, -8);
                Logger.Info($"Starting browser with resolution {_driver.Manage().Window.Size}");

                // Selenium requires opening a page to add cookies (e.g. for session)
                _driver.Navigate().GoToUrl(TestData.Get.Env.BaseUiUri);
            }
            catch (Exception)
            {
                TakeScreenshot();
                Dispose();
                throw;
            }
        }

        public void ResetSession(string sessionId = null)
        {
            _driver.Manage().Cookies.DeleteAllCookies();
            if (sessionId == null) return;

            var seleniumCookie = new Cookie(AuthenticationManager.SessionCookieName, sessionId, TestData.Get.Env.RootDomain, "/", DateTime.MaxValue);
            _driver.Manage().Cookies.AddCookie(seleniumCookie);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _driver.Quit();
            }
        }

        protected override void TakeScreenshotInternal()
        {
            lock (BrowserLock)
            {
                try
                {
                    var ss = ((ITakesScreenshot)_driver).GetScreenshot();
                    ss.SaveAsFile(ScreenshotFile, ScreenshotImageFormat.Png);
                }
                catch (Exception e)
                {
                    Logger.Info($"Exception while taking screenshot: \n {e.Message} \n {e.StackTrace}");
                }
            }
        }

        public void ExecuteJavaScript(string script)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript(script);
        }

        public void NavigateToUrl(Uri url)
        {
            Logger.Info("Navigate to url: " + url);
            _driver.Navigate().GoToUrl(url);
            _driver.Navigate().Refresh();
            WaitForLoadingToFinish();
        }

        public String GetCurrentUrl()
        {
            return _driver.Url;
        }

        public void Click(string selector) => ClickInternal(By.CssSelector(selector));
        public void ClickByXPath(string xPath) => ClickInternal(By.XPath(xPath));
        private void ClickInternal(By by)
        {
            Logger.Debug("Focus and click element by: " + by);
            Retry.Execute(Logger, () =>
            {
                var element = _driver.FindElement(by);
                new Actions(_driver).MoveToElement(element).Perform();
                WaitForLoadingToFinish();
                element.Click();
            });
        }

        public void Enter(string selector, string text) => EnterInternal(By.CssSelector(selector), text);
        public void EnterByXPath(string xPath, string text) => EnterInternal(By.XPath(xPath), text);
        private void EnterInternal(By by, string text)
        {
            Logger.Debug("Enter text [" + text + "] to element by: " + by);
            Retry.Execute(Logger, () =>
            {
                var element = _driver.FindElement(by);
                element.Clear();
                element.SendKeys(text);
            });
            WaitForLoadingToFinish();
        }

        public string GetTextFromElement(string selector) => GetTextFromElementInternal(By.CssSelector(selector));
        public string GetTextFromElementByXPath(string xPath) => GetTextFromElementInternal(By.XPath(xPath));
        private string GetTextFromElementInternal(By by)
        {
            Logger.Debug("Get text from element by " + by);
            var result = Retry.Execute(Logger, () => _driver.FindElement(by).Text);
            Logger.Debug($"Text from element is: '{result}'");
            return result;
        }

        public void SelectDropdownValueByXPath(string xPath, string value) => SelectDropdownByValueInternal(By.XPath(xPath), value);
        private void SelectDropdownByValueInternal(By dropdownSelectLocator, string value)
        {
            Logger.Debug("Select value [" + value + "] in dropdown by: " + dropdownSelectLocator);
            Retry.Execute(Logger, () =>
            {
                var selectElement = new SelectElement(_driver.FindElement(dropdownSelectLocator));
                selectElement.SelectByValue(value);
            });
            WaitForLoadingToFinish();
        }

        public void SelectDropdownText(string selector, string text) => SelectDropdownByTextInternal(By.CssSelector(selector), text);
        private void SelectDropdownByTextInternal(By dropdownSelectLocator, string text)
        {
            Logger.Debug("Select text [" + text + "] in dropdown by: " + dropdownSelectLocator);
            Retry.Execute(Logger, () =>
            {
                var selectElement = new SelectElement(_driver.FindElement(dropdownSelectLocator));
                selectElement.SelectByText(text);
            });
            WaitForLoadingToFinish();
        }

        public string GetSelectedDropdownValue(string selector)
        {
            var result = string.Empty;
            Logger.Debug("Get selected value in dropdown by: " + selector);
            Retry.Execute(Logger, () =>
            {
               var selectElement = new SelectElement(_driver.FindElement(By.CssSelector(selector)));
               result = selectElement.SelectedOption.GetAttribute("value");
            });
            return result;
        }

        public void WaitUntilElementVisible(string selector) => WaitUntilElementVisibleInternal(By.CssSelector(selector));
        public void WaitUntilElementVisibleByXPath(string xPath) => WaitUntilElementVisibleInternal(By.XPath(xPath));
        private void WaitUntilElementVisibleInternal(By by, TimeSpan? timeToWait = null)
        {
            var wait = timeToWait ?? WaitUntilElementAppearTimeoutInSeconds;

            Logger.Debug($"Wait up to {wait.Seconds} seconds until element appears and visible. Element by:  {by}");
            var webDriverWait = new WebDriverWait(_driver, wait);
            webDriverWait.Message = $"Element {by} not present or visible after {webDriverWait.Timeout.Seconds} seconds";
            webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(by));
            webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
        }

        public override void WaitUntilElementHidden(string selector) => WaitUntilElementHiddenInternal(By.CssSelector(selector));
        private void WaitUntilElementHiddenInternal(By by)
        {
            Logger.Debug($"Wait up to {WaitUntilElementAppearTimeoutInSeconds.Seconds} seconds until element disappeared. Element by:  {by}");
            var webDriverWait = new WebDriverWait(_driver, WaitUntilElementAppearTimeoutInSeconds);
            webDriverWait.Message = $"Element {by} not disappeared after {webDriverWait.Timeout.Seconds} seconds";
            webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(by));
        }
        
        public bool IsElementVisible(string selector, TimeSpan? timeToWait = null) => IsElementVisibleInternal(By.CssSelector(selector), timeToWait);

        public bool IsElementVisibleByXPath(string xPath) => IsElementVisibleInternal(By.XPath(xPath));
        private bool IsElementVisibleInternal(By by, TimeSpan? timeToWait = null)
        {
            Logger.Info($"Is element '{by}' present ? ");
            try
            {
                WaitUntilElementVisibleInternal(by, timeToWait);
                return true;
            }
            catch (Exception e)
            {
                Logger.Info($"Element '{by}' is not visible. {e.Message}");
                return false;
            }
        }

        public string GetClassNameByXPath(string xPath) => GetElementAttributeInternal(By.XPath(xPath), "class");

        public string GetElementAttribute(string selector, string attributeName) => GetElementAttributeInternal(By.CssSelector(selector), attributeName);
        public string GetElementAttributeByXPath(string xpath, string attributeName) => GetElementAttributeInternal(By.XPath(xpath), attributeName);

        private string GetElementAttributeInternal(By by, string attributeName)
        {
            Logger.Debug("Get attribute '{attributeName}' from element by " + by);
            var result = Retry.Execute(Logger, () => _driver.FindElement(by).GetAttribute(attributeName));
            Logger.Debug($"Attribute '{attributeName} is: '{result}'");
            return result;
        }

        public int GetElementCount(string selector) => GetElementCountInternal(By.CssSelector(selector));
        public int GetElementCountByXPath(string xPath) => GetElementCountInternal(By.XPath(xPath));
        private int GetElementCountInternal(By by)
        {
            Logger.Debug($"Count quantity of elements by: {by}");
            _driver.Manage().Timeouts().ImplicitWait = WaitUntilElementAppearTimeoutInSeconds;
            var quantity = _driver.FindElements(by).Count;
            Logger.Debug("Found " + quantity + " elements");
            //set implicit wait back to 0, as it can affect some waiting
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            return quantity;
        }

        public void SwitchToIframeByXPath(string xPath) => SwitchToIframeInternal(By.XPath(xPath));
        private void SwitchToIframeInternal(By by)
        {
            Logger.Info($"Switching to iframe by: {by}");
            Retry.Execute(Logger, () => { _driver.SwitchTo().Frame(_driver.FindElement(by)); });
            WaitForLoadingToFinish();
        }

        public void ReloadBrowserPage()
        {
            Logger.Info("Reloading browser page...");
            _driver.Navigate().Refresh();
            WaitForLoadingToFinish();
        }
    }
}
