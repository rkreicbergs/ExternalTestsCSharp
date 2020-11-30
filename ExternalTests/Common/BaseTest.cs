using System;
using ExternalTests.Data;
using Xunit.Abstractions;

namespace ExternalTests.Common
{
    public abstract class BaseTest : IDisposable
    {
        protected BaseContext Context { get; }

        protected User User => Context.User;

        protected string SessionId => Context.SessionId;

        protected RequestWrapper RequestWrapper => Context.RequestWrapper;

        protected Logger Logger => Context.Logger;

        protected BaseTest(ITestOutputHelper output, BaseContext context)
        {
            Context = context;
            context.SetTestName(output);
        }

        protected BaseTest(ITestOutputHelper output) : this(output, new BaseContext())
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
