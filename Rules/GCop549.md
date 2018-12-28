# GCop 549

> *"Use lambda expression instead of anonymous method."*

## Rule description

In C#3.0 Lambda expressions are introduced. It provides a simple, more concise, functional syntax to write anonymous methods rather than using `delegate`.

## Example

```csharp
foo = items.Select(delegate (object bar)
{
    return bar.ToString();
});
```

*should be* 🡻

```csharp
foo = items.Select((object bar) =>
{
    return bar.ToString();
});
```

