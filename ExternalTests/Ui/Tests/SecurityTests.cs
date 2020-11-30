using ExternalTests.Common;
using Xunit;
using Xunit.Abstractions;

namespace ExternalTests.Ui.Tests
{
    public class XssUi : BaseUiTest
    {
        public XssUi(ITestOutputHelper output) : base(output)
        {
        }

        [Fact, Stage, Production]
        public void SimpleXssProtection()
        {
        }
    }
}