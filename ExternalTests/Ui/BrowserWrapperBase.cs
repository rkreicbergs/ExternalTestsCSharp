using System;
using System.IO;
using ExternalTests.Common;
using ExternalTests.Data;

namespace ExternalTests.Ui
{
    public abstract class BrowserWrapperBase : IDisposable
    {
        protected static readonly TimeSpan WaitUntilElementAppearTimeoutInSeconds = TimeSpan.FromSeconds(WaitTimeoutInSeconds);
        protected static readonly object BrowserLock = new object();

        protected readonly Logger Logger;
        protected string ScreenshotFile { get; private set;  }

        private const int WaitTimeoutInSeconds = 15;
        private static readonly ConcurrentHashSet<string> CreatedScreenshotFiles = new ConcurrentHashSet<string>();
        private static readonly string ScreenshotFolder = Path.Combine(TestDataHelper.GetOutputDirectory, "Screenshots");

        protected BrowserWrapperBase(Logger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void SetScreenshotFileName(string testName)
        {
            var res = Directory.CreateDirectory(ScreenshotFolder);
            Logger.Debug($"Created directory {ScreenshotFolder}. Name: {res.Name}. Exists: {res.Exists}. Full name: {res.FullName}");

            ScreenshotFile = Path.Combine(ScreenshotFolder, TestDataHelper.GetFileNameForTest(testName, ".png"));
            lock (BrowserLock)
            {
                if (File.Exists(ScreenshotFile))
                {
                    //delete only files that are not created by test session
                    if (!CreatedScreenshotFiles.Contains(ScreenshotFile))
                    {
                        if (File.Exists(ScreenshotFile))
                        {
                            File.Delete(ScreenshotFile); Logger.Debug($"Deleting screenshot file {ScreenshotFile}");
                        }
                    }
                    CreatedScreenshotFiles.Add(ScreenshotFile);
                }
            }
            Logger.Debug($"Set screenshot file to {ScreenshotFile}");
        }

        public void Dispose()
        {
            Logger.Info($"Disposing {GetType().FullName}...");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
        protected abstract void TakeScreenshotInternal();
        public abstract void WaitUntilElementHidden(string selector);

        public void TakeScreenshot()
        {
            if (!Convert.ToBoolean(TestSettings.Get.SeleniumSettings.TakeScreenshots)) return;

            Logger.Info("Taking screenshot: " + ScreenshotFile);
            TakeScreenshotInternal();
        }
        
        protected void WaitForLoadingToFinish()
        {
            // WaitUntilElementHidden("some loading widget selector");
        }
    }
}