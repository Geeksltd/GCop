# GCop 624

> *"Write it as `foo.Except(VALUE).Any()`"*

## Rule description

The `Except()` method subtracts elements from a collection. It essentially subtracts all the elements in one collection from another. It is more readable to use `Except` than using negative queries.

## Example

```csharp
if(foo.Any(fo => fo != bar))
{
    ...
}
```

*should be* 🡻

```csharp
if(foo.Except(bar).Any())
{
    ...
}
```
