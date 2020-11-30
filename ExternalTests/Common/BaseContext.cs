using System.Reflection;
using ExternalTests.Data;
using Xunit.Abstractions;

namespace ExternalTests.Common
{
    public class BaseContext
    {
        public User User { get; private set; }

        public string SessionId { get; private set; }

        public RequestWrapper RequestWrapper { get; private set; }

        public Logger Logger { get; }

        protected string TestName { get; private set; }

        public BaseContext()
        {
            Logger = new Logger();
            Logger.SetLogFileName(GetType().Name);
        }

        public virtual void SetTestName(ITestOutputHelper output)
        {
            var type = output.GetType();
            var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            var test = (ITest)testMember.GetValue(output);
            TestName = test.DisplayName;
            Logger.SetLogFileName(TestName);
        }

        public virtual void LoginWithUser(string user_id = "base_user")
        {
            User = TestData.Get.GetUser(user_id);
            SessionId = AuthenticationManager.GetSessionId(Logger, User);
            Logger.Info($"Logged in with user '{User.username}'");
            
            RequestWrapper = new RequestWrapper(Logger, TestData.Get.Env.BaseUiUri, SessionId);
        }
    }
}