# GCop 131

> *"Use \{collectionName}.ToString(\{seperator}) instead of \{string.Join}(\{seperator}, \{collectionName})"*

## Rule description

The `string.Tostring("separator")` places the separator between every element of the collection in the returned string. The separator is not added to the start or end of the result. It is just like `string.join()` but briefer and more readable.

## Example

```csharp
var result = string.Join(",", myStringArray);
```

*should be* 🡻

```csharp
var result = myStringArray.ToString(",");
```