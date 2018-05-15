# GCop 155

> *"Should be written simply as {0}.Intersect({1})"*

## Rule description

The `Intersect` method returns the common elements of both entities and returns the result as a new entity. It is more faster than using `Where` with `Contains`.

## Example

```csharp
var result = myCollection.Where(s => anotherCollection.Contains(s));
```

*should be* 🡻

```csharp
var result = myCollection.Intersect(anotherCollection);
```
