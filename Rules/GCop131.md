# GCop131

> *"Use StrinArray.ToString(separator) instead of string.Join(separator, StrinArray)"*


## Rule description
Using Tostring(seperator) is more meaningful rather than string.Join().

## Example 1
```csharp
string[] arr = { "one", "two", "three" };
Console.WriteLine(string.Join(",", arr));
```
*should be* 🡻

```csharp
string[] arr = { "one", "two", "three" };
Console.WriteLine(arr.ToString(","));
```

