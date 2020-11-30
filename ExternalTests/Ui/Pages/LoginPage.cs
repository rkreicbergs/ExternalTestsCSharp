using System;
using ExternalTests.Data;

namespace ExternalTests.Ui.Pages
{
    public class LoginPage : BasePage
    {
        public override Uri UrlForPage => TestData.Get.GetUrl("/login");

        public void TryLogin(User user)
        {
            Browser.Enter("input[name='username']", user.username);
            Browser.Enter("input[name='password']", user.password);
            Browser.Click("button.loginbtn");
        }
    }
}
