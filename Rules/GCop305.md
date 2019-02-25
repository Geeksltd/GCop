# GCop 305

> *"Replace with `Database.Find<{T}>({LambdaExpression})`"*

## Rule description

The `.Find()` method returns first matched record of a specified Entity type if found. It evaluates the criteria at the database level and runs much faster than if you fetch all records from the database into the .NET process and then find the match using `FirstOrDefault()`.

## Example

```csharp
var result = Database.GetList<Customer>().FirstOrDefault(s => s.Number = someValue);
```

*should be* 🡻

```csharp
var result = Database.Find<Customer>(s => s.Number = someValue);
```
