# GCop 183

> *"Use AttributeUsage Attribute"*

## Rule description

A custom attribute declaration begins with the `System.AttributeUsageAttribute`, which defines some of the key characteristics of your attribute class. The `AttributeUsageAttribute` has three members that are important for the creation of custom attributes. It is preferred to declare these members to have a better code design, while without declaring, the default values are indicates.

## Example

```csharp
public class FooAttribute : Attribute
{
    ...
}
```

*should be* 🡻

```csharp
[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class FooAttribute : Attribute
{
    ...
}
```

