# GCop 130

> *"Instead of ?? Enumerable.Empty<>() use .OrEmpty() for collections that can be null"*

## Rule description

The `Enumerable.Empty<TResult>()` method returns an empty collection that has the specified type argument. You can use this method with conditional operator `??`  To return an empty collection when your collection is null, or a better solution, use `OrEmpty()` method. This method do the same but is more readable and easy to use. 

## Example

```csharp
var result = myCollection ?? Enumerable.Empty<TResult>();
```

*should be* 🡻

```csharp
var result = myCollection.OrEmpty();
```