# GCop 533

> *"Choose return type of IEnumerable<\{entityType}> to imply that it's not modifiable."*

## Rule description

When your code only needs to read data and return that, use `IEnumerable` type rather than `List`. `IEnumerable<T>` is nice to use when you want to represent sequence of items, that you can iterate over, but you don't want to allow modifications(Add, Delete etc).

## Example

```csharp
public List<Settings> GetSettings()
{
    // ... code that only return Settings
}
```

*should be* 🡻

```csharp
public IEnumerable<Settings> GetSettings()
{
    // ... code that only return Settings
}
```