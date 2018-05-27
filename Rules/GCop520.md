# GCop 520

> *"Write it as `{StringListName}.Intersects({AnotherStringListName})`"*

## Rule description

The `Intersect` method returns the common elements between two collections. It is faster and more readable than a manual implementation of that logic using `Where` with `Contains`.

## Example
> It should be tested after release. See https://github.com/Geeksltd/GCop/issues/153
```csharp
someStringList.Any(rec => anotherStringList.Contains(rec));
```

*should be* 🡻

```csharp
someStringList.Intersects(anotherStringList);
```