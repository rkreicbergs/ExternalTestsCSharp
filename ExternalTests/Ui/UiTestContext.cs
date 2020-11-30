using System;
using ExternalTests.Common;
using Xunit.Abstractions;

namespace ExternalTests.Ui
{
    public class UiTestContext : BaseContext, IDisposable
    {
        public SeleniumBrowserWrapper Browser { get; }

        public UiTestContext()
        {
            Browser = new SeleniumBrowserWrapper(Logger);
        }

        public override void SetTestName(ITestOutputHelper output)
        {
            base.SetTestName(output);
            Browser.SetScreenshotFileName(TestName);
        }

        public override void LoginWithUser(string user_id = "base_user")
        {
            base.LoginWithUser(user_id);
            Browser.ResetSession(SessionId);
        }

        public void Dispose()
        {
            Browser.Dispose();
        }

        protected void SetupSafe(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                Browser.SetScreenshotFileName(GetType().ToString());
                Browser.TakeScreenshot();
                Dispose();
                throw;
            }
        }
    }
}