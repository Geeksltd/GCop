# GCop 640

> *"Write it as `foo.Contains(VALUE)`"*

## Rule description

The `someCollection.Contains(...)` method takes an object while `Any(...)` takes a predicate. So if you want to check for existence of an element, use `Contains(...)` rather than comparing every item using `Any(...)`.

## Example

```csharp
if(foo.Any(fo => fo == bar))
{
    ...
}
```

*should be* 🡻

```csharp
if(foo.Contains(fo => fo == bar))
{
    ...
}
```