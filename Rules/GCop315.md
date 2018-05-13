# GCop 315

> *"It should be written instead as: `Database.Count<{T}>{(LambdaExpression)}`"*

## Rule description

The `Count()` method is used to determine the number of elements in any sequence and there is no need to use `Getlist()` method. In this way you can have a more readable code and improve its performance.

## Example

```csharp
var customerCount = Database.GetList<Customer>(s => s.PurchaseCount > someValue ).Count();
```

*should be* 🡻

```csharp
var customerCount = Database.Count<Customer>(s => s.PurchaseCount > someValue );
```