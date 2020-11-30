using System;
using ExternalTests.Data;

namespace ExternalTests.Ui.Pages
{
    public class AccountPage : BasePage
    {
        public override Uri UrlForPage => TestData.Get.GetUrl("/account/");

        public void WaitTillAccountPageLoaded(User user)
        {
            Browser.WaitUntilElementVisibleByXPath($"//h3[contains(text(),'Hi, {user.name} {user.surname}')]");
        }        
    }
}