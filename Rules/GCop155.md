# GCop 155

> *"Should be written simply as `{CollectionName}.Intersect({AnotherCollectionName})`"*

## Rule description

The `Intersect` method returns the common elements between two collections. It is faster and more readable than a manual implementation of that logic using `Where` with `Contains`.

## Example

```csharp
var result = myCollection.Where(s => anotherCollection.Contains(s));
```

*should be* 🡻

```csharp
var result = myCollection.Intersect(anotherCollection);
```
