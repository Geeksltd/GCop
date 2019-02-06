# GCop 313

> *"`Where` should be called first, so it is not doing unnecessary ordering of objects that will be thrown away."*

## Rule description

Using `Where` clause after `OrderBy` clause requires the whole collection to be sorted and then filtered. If we had a million items, only one of which equals to `Where` condition, we'd be wasting a lot of time ordering results which would be thrown away. So `Where` should be called first.

## Example

```csharp
var result = foo.OrderBy(o => o.Bar).Where(a => a.Id == id).ToList();
```

*should be* 🡻

```csharp
var result = foo.Where(a => a.Id == id).OrderBy(o => o.Bar).ToList();
```
