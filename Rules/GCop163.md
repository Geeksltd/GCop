# GCop 163

> *"Subsequent OrderBy or OrderByDescending cancel each other out. Instead ThenBy or ThenByDescending should be called."*

## Rule description

If you call `OrderBy` multiple times, it will effectively reorder the sequence completely several times, so the final call will effectively be the dominant one. The point of `OrderBy` is to provide the "most important" ordering projection; then use `ThenBy` (repeatedly) to specify secondary, tertiary etc ordering projections.

## Example

```csharp
var result = myCollection.OrderBy(or => or.SomeElement).OrderBy(or => or.AnotherElement).ToList();
```

*should be* 🡻

```csharp
var result = myCollection.OrderBy(or => or.SomeElement).ThenBy(or => or.AnotherElement).ToList();
```