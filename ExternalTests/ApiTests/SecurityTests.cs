using ExternalTests.Common;
using Xunit;
using Xunit.Abstractions;

namespace ExternalTests.ApiTests
{
    public class SecurityTests : BaseTest
    {
        public SecurityTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact, Stage]
        public void SuccessfulPostWithCsrfProtection()
        {
        }

        [Fact, Stage]
        public void StoppedByCsrfProtection()
        {
        }
    }
}