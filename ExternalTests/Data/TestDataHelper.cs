using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ExternalTests.Data
{
    public static class TestDataHelper
    {
        private static string GetOutputDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [Serializable]
        private class EnvironmentsConfig
        {
            public Dictionary<string, Environment> Environments { get; set; }
        }

        internal static TestData LoadTestData()
        {
            var environmentData = GetEnvironmentData();
            var env = environmentData.Environments[TestSettings.Get.Environment];

            var testDataJsonFile = $@"{GetOutputDirectory}\TestData\Resources\TestData.{env.EnvironmentType}.json";
            using var r = new StreamReader(testDataJsonFile);
            var json = r.ReadToEnd();
            var testData = JsonConvert.DeserializeObject<TestData>(json);
            testData.Env = env;
            return testData;
        }

        private static EnvironmentsConfig GetEnvironmentData()
        {
            var environmentsJsonFile = $@"{GetOutputDirectory}\TestData\Resources\Environments.json";
            using var r = new StreamReader(environmentsJsonFile);
            var json = r.ReadToEnd();
            var t = JsonConvert.DeserializeObject<EnvironmentsConfig>(json);
            return t;
        }
    }
}
