# GCop 131

> *"Use `collection.ToString("separator")` instead of `string.Join("separator", collection)"*

## Rule description

The `string.Tostring("separator")` places the separator between every element of the collection in the returned string. The separator is not added to the start or end of the result. It is just like `string.join()` but briefer and more readable.

## Example

```csharp
var result = string.Join(",", myCollection);
```

*should be* 🡻

```csharp
var result = myCollection.ToString(",");
```