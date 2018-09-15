# GCop 659

> *"Use `var` instead of explicit type."*

## Rule description

The potential benefit of using `var` instead of type is in readability and brevity. It can be used within a `foreach` loop as an iterator for cycling through the results.

## Example

```csharp
foreach (string item in items)
{
    ...
}
```

*should be* 🡻

```csharp
foreach (var item in items)
{
    ...
}
```

