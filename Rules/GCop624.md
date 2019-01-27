# GCop 624

> *"Write it as `foo.Contains(VALUE)`"*

## Rule description

The `someCollection.Contains(...)` method takes an object while `Any(...)` takes a predicate. So if you want to check for existence of an element, use `Contains(...)` rather than comparing every item using `Any(...)`.

## Example

```csharp
var res = foo.Any(fo => fo == bar);
```

*should be* 🡻

```csharp
var res = foo.Contains(fo => fo == bar);
```
