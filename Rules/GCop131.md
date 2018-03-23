# GCop131

> *"Use string[].ToString(separator) instead of string.join(separator, string[])"*


## Rule description
...

## Example 1
```csharp
string[] stringArray = { "one", "two", "three" };
var commaSeperated = string.Join(",", stringArray);
```
*should be* 🡻

```csharp
string[] stringArray = { "one", "two", "three" };
var decimalvar = stringArray.ToString(",");
```

