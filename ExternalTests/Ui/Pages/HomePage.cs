using System;
using ExternalTests.Data;

namespace ExternalTests.Ui.Pages
{
    public class HomePage : BasePage
    {
        public override Uri UrlForPage => TestData.Get.GetUrl("/home/");

        public void WaitTillHomePageLoadedForUser(User user)
        {
            Browser.WaitUntilElementVisibleByXPath($"//a[contains(.,'{user.name}')]");
        }        
    }
}