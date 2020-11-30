using ExternalTests.Common;
using Xunit;
using Xunit.Abstractions;

namespace ExternalTests.ApiTests
{
    public class Login : BaseTest
    {
        public Login(ITestOutputHelper output) : base(output)
        {
        }

        [Fact, Stage, Production] // trait(Environment=Production)
        public void Successful()
        {
            Context.LoginWithUser();
        }

        [Fact, Stage]
        public void Failed()
        {
            Assert.Throws<Xunit.Sdk.EqualException>(() => Context.LoginWithUser("wrong_password_user"));
        }
    }
}
