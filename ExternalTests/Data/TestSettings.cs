using Microsoft.Extensions.Configuration;

namespace ExternalTests.Data
{
    public class TestSettings
    {
        public static TestSettings Get { get; } = LoadTestSettings();
        
        public string Environment { get; set; }
        public SeleniumSettings SeleniumSettings { get; set; }

        private static TestSettings LoadTestSettings()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var settings = new TestSettings();
            configuration.Bind(settings);
            return settings;
        }
    }
}