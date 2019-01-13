# GCop 522

> *"Avoid `NullReferenceException`."*

## Rule description

`NullReferenceException` indicates that you are trying to access member fields, or function types, on an object reference that points to null. C# 6.0 has introduced the null propagation operator (`?.`) that enables check for the null value within an object reference chain. This will return null if anything in the object reference chain is null. 

## Example1

```csharp
private void Bar(Dictionary<string, string> foo)
{
    var result = foo.Keys.FirstOrDefault();
}
```

*should be* 🡻

```csharp
private void Bar(Dictionary<string, string> foo)
{
    var result = foo?.Keys?.FirstOrDefault();
}
```

## Example2

```csharp
private void Bar(List<int?> foo)
{
    var result = foo.FirstOrDefault();
}
```

*should be* 🡻

```csharp
private void Bar(Dictionary<string, string> foo)
{
    var result = foo?.FirstOrDefault();
}
```

