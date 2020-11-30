using System;
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;
namespace ExternalTests.Common
{
    [TraitDiscoverer("ExternalTests.Common.EnvironmentDiscoverer", "ExternalTests")]
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class EnvironmentAttribute : Attribute, ITraitAttribute
    {
        public abstract string Environment { get; }
    }

    public class StageAttribute : EnvironmentAttribute
    {
        public override string Environment => "Stage";
    }

    public class ProductionAttribute : EnvironmentAttribute
    {
        public override string Environment => "Production";
    }


    public class EnvironmentDiscoverer : ITraitDiscoverer
    {
        private const string Key = "Environment";
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var attributeInfo = traitAttribute as ReflectionAttributeInfo;
            var category = attributeInfo?.Attribute as EnvironmentAttribute;
            var value = category?.Environment ?? string.Empty;
            yield return new KeyValuePair<string, string>(Key, value);
        }
    }
}
