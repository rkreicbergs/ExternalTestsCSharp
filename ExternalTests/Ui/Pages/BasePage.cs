using System;
using ExternalTests.Common;

namespace ExternalTests.Ui.Pages
{
    public abstract class BasePage
    {
        protected SeleniumBrowserWrapper Browser { get; private set; }
        protected Logger Logger { get; private set; }
        public virtual Uri UrlForPage => throw new InvalidOperationException("This page does not have associated URL");

        public void SetLogger(Logger logger, SeleniumBrowserWrapper browser)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Browser = browser ?? throw new ArgumentNullException(nameof(browser));
        }
    }
}
