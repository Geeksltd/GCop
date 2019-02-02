# GCop 690

> *"Negative logic is taxing on the brain. Use `foo == null` instead."*

## Rule description

Human brain can understand positive expressions and statements faster than negative ones. To improve code readability it’s better to check for `null` rather than negative `HasValue`. 

## Example

```csharp
public void Bar(int? foo)
{
    if (!foo.HasValue)
    {
        ...
    }
}
```

*should be* 🡻

```csharp
public void Bar(int? foo)
{
    if (foo == null)
    {
        ...
    }
}
```