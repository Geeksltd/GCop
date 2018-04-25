# GCop 313

> *"Where should be called first, so it is not doing unnecessary ordering of objects that will be thrown away."*

## Rule description

Using `Where` clause after `OrderBy` clause requires the whole collection to be sorted and then filtered. If we had a million items, only one of which equals to `Where` condition, we'd be wasting a lot of time ordering results which would be thrown away. So `Where` should be called first.

## Example

```csharp
var result = myCollection.OrderBy(or => or.SomeElement).Where(a => a.Id == id).ToList();
```

*should be* 🡻

```csharp
var result = myCollection.Where(a => a.Id == id).OrderBy(or => or.SomeElement).ToList();
```
