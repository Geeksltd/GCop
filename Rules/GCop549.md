# GCop 549

> *"Use lambda expression instead of anonymous method."*

## Rule description

In C#3.0 Lambda expressions are introduced. It provides a simple, more concise, functional syntax to write anonymous methods rather than using `delegate`.

## Example

```csharp
var someVar = items.Select(delegate (object someObj)
{
    return someObj.ToString();
});
```

*should be* 🡻

```csharp
var someVar = items.Select((object someObj) =>
{
    return someObj.ToString();
});
```

