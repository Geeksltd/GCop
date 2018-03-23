# GCop124

> *"Use **numeric string.TryParseAs< data type >()** instead of **data type.TryParse(numeric string)**"*


## Rule description
...

## Example 1
```csharp
decimal myDecimalVar = 0;
decimal.TryParse("35" + "4",out myDecimalVar);
```
*should be* 🡻

```csharp
var myDecimalVar = "35" + "4".TryParseAs<decimal>();
```

