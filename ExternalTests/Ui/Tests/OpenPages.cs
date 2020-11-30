using ExternalTests.Common;
using ExternalTests.Ui.Pages;
using Xunit;
using Xunit.Abstractions;

namespace ExternalTests.Ui.Tests
{
    public class ReusableContext : UiTestContext
    {
        public ReusableContext()
        {
            SetupSafe(() =>
            {
                LoginWithUser("base_user");
            });
        }
    }

    /// <summary>
    /// Log in once for multiple tests 
    /// </summary>
    public class OpenPages : BaseUiTest, IClassFixture<ReusableContext>
    {
        public OpenPages(ITestOutputHelper output, ReusableContext context) : base(output, context, loginWithBaseUser: false)
        {
        }

        [Fact, Stage, Production]
        public void Account()
        {
            var accountPage = GoToPage<AccountPage>();
            accountPage.WaitTillAccountPageLoaded(Context.User);
        }

        [Fact, Stage, Production]
        public void HomePage()
        {
            var homePage = GoToPage<HomePage>();
            homePage.WaitTillHomePageLoadedForUser(Context.User);
        }
    }
}