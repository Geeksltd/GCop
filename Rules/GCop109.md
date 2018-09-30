# GCop 109

> *Use `bars.HasMany()` instead of `bars.Count() > 1`, as it will be faster and more descriptive."*

## Rule description

The `Count()` method can potentially be time consuming, especially if the `IEnumerable` object is lazy evaluated such as with Linq expressions. The `HasMany()` method has a more efficient implementation and doesn't have to count all items. As soon as it finds more than one, it returns `true`.

## Example

```csharp
IEnumerable<Bar> bars = ...;
if (bars.Count() > 1)
{
    ...
}
```

*should be* 🡻

```csharp
IEnumerable<Bar> bars = ...;
if (bars.HasMany())
{
    ...
}
```