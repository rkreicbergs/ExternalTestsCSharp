using System;
using ExternalTests.Common;
using ExternalTests.Data;
using ExternalTests.Ui.Pages;
using Xunit;
using Xunit.Abstractions;

namespace ExternalTests.Ui.Tests
{
    public abstract class BaseUiTest : BaseTest
    {
        protected new UiTestContext Context { get; }

        protected SeleniumBrowserWrapper Browser { get; }

        private readonly bool _disposeContext;

        protected BaseUiTest(ITestOutputHelper output, UiTestContext context, bool loginWithBaseUser = true, bool disposeContext = false) : base(output, context)
        {
            Context = context; // Hides Context from base (different type)
            Browser = Context.Browser;
            _disposeContext = disposeContext;

            if (loginWithBaseUser)
            {
                try
                {
                    Context.LoginWithUser("base_user");
                }
                catch (Exception)
                {
                    Dispose();
                    throw;
                }
            }
        }

        protected BaseUiTest(ITestOutputHelper output, bool loginWithBaseUser = true)
            : this(output, new UiTestContext(), loginWithBaseUser, disposeContext: true)
        {
        }

        public override void Dispose()
        {
            Browser.TakeScreenshot();
            if (_disposeContext)
            {
                Context.Dispose();
            }
            base.Dispose();
        }

        /// <summary>
        /// Used when you want to manually navigate to a new page
        /// </summary>
        protected TPageType GoToPage<TPageType>() where TPageType : BasePage, new()
        {
            var page = new TPageType();
            page.SetLogger(Logger, Browser);
            var pageUrl = TestData.Get.GetUrl(page.UrlForPage);
            Browser.NavigateToUrl(pageUrl);
            return page;
        }

        /// <summary>
        /// Used when you are redirected to a page
        /// </summary>
        protected TPageType GetPage<TPageType>() where TPageType : BasePage, new()
        {
            var page = new TPageType();
            page.SetLogger(Logger, Browser);

            Retry.Execute(Logger, () =>
            {
                var currentUrl = Browser.GetCurrentUrl();
                var pageUrl = TestData.Get.GetUrl(page.UrlForPage);
                Assert.Equal(pageUrl.ToString(), currentUrl);
            });

            return page;
        }
    }
}
