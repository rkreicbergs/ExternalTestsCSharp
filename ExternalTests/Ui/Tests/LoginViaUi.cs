using ExternalTests.Data;
using ExternalTests.Ui.Pages;
using Xunit;
using Xunit.Abstractions;

namespace ExternalTests.Ui.Tests
{
    /// <summary>
    /// Multiple tests without a common setup 
    /// </summary>
    public class LoginViaUi : BaseUiTest
    {
        public LoginViaUi(ITestOutputHelper output) : base(output, loginWithBaseUser: false)
        {
        }

        [Fact]
        public void LoginViaUiTest()
        {
            var user = TestData.Get.GetUser("base_user");
            var loginPage = GoToPage<LoginPage>();
            loginPage.TryLogin(user);

            var accountPage = GetPage<AccountPage>();
            accountPage.WaitTillAccountPageLoaded(user);
        }

        [Fact]
        public void RedirectWithoutLogin()
        {
            // Just in case, clear cookies first
            Browser.ResetSession();
            Browser.NavigateToUrl(TestData.Get.GetUrl("/account/"));

            var loginPage = GetPage<LoginPage>();
        }
    }
}