using System;
namespace KR.Common.Attributes.Persistance;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class DeepObjectAttribute : Attribute
{
    public String DeepMap { get; private set; }
    public String Alias { get; private set; }

    public DeepObjectAttribute(string deepMap, string alias = null)
    {
        DeepMap = deepMap;
        Alias = alias;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class IgnoreAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public class DBConfigurationAttribute : Attribute
{
}