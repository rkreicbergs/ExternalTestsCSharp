using System;

namespace ExternalTests.Data
{
    public class Environment
    {
        public EnvironmentType EnvironmentType { get; set; }
        public Uri BaseUiUri { get; set; }
        public string RootDomain { get; set; }
    }
}