using ExternalTests.Common;
using Xunit;
using Xunit.Abstractions;

namespace ExternalTests.ApiTests
{
    public class SimpleTest : BaseTest
    {
        public SimpleTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact, Stage, Production]
        public void SimpleApiTest()
        {
        }
    }
}
