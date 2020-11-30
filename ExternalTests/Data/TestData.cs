using System;
using System.Collections.Generic;
using System.Linq;

namespace ExternalTests.Data
{
    public class TestData
    {
        public static TestData Get { get; } = TestDataHelper.LoadTestData();

        public Environment Env  { get; set; }
        public List<User> Users { get; set; }

        public User GetUser(string user_id)
        {
            return Users.SingleOrDefault(p => p.user_id.Equals(user_id)) ?? throw new Exception($"User '{user_id}' not found");
        }

        public Uri GetUrl(Uri relativeUrl)
        {
            return new Uri(Env.BaseUiUri, relativeUrl);
        }

        public Uri GetUrl(string relativeUrl)
        {
            return new Uri(Env.BaseUiUri, relativeUrl);
        }
    }
}
