using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using ExternalTests.Data;
using RestSharp;
using Xunit;

namespace ExternalTests.Common
{
    public static class AuthenticationManager
    {
        public const string SessionCookieName = "ci_session";
        private static readonly ConcurrentDictionary<string, string> SessionIds = new ConcurrentDictionary<string, string>();

        public static string GetSessionId(Logger logger, User user)
        {
            if (SessionIds.ContainsKey(user.user_id))
            {
                logger.Info($"Reuse session for user: {user.user_id} ({user.username})");
                return SessionIds[user.user_id];
            }

            var req = new RestRequest("/account/login") { Method = Method.POST };
            req.AddParameter("username", user.username);
            req.AddParameter("password", user.password);
            req.AddHeader("x-requested-with", "XMLHttpRequest"); // Apparently, this header is needed to get content

            var httpRequest = new RequestWrapper(logger, TestData.Get.Env.BaseUiUri);
            var response = httpRequest.ExecuteRequest(req);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("true", response.Content);

            var cookies = response.Cookies.ToDictionary(c => c.Name);
            var sessionId = cookies[SessionCookieName].Value;
            SessionIds.TryAdd(user.user_id, sessionId);
            return sessionId;
        }
    }
}