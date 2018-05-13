# GCop 305

> *"Error messageReplace with `Database.Find<{T}>({LambdaExpression})`"*

## Rule description

The `.Find()` method returns first matched record of the provided Entity Type if found and returns `null` if no record is available in database based on the provided criteria. So there is no need to use `GetList()` with `FirstOrDefault()` method, while these methods can decrease performance.

## Example

```csharp
var result = Database.GetList<Customer>().FirstOrDefault(s => s.Number = someValue);
```

*should be* 🡻

```csharp
var result = Database.Find<Customer>(s => s.Number = someValue);
```