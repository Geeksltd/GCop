# GCop 315

> *"It should be written instead as: `Database.Count<{T}>{(LambdaExpression)}`"*

## Rule description

The `Count()` method on the `Database` class is used to determine the number of records that match a criteria. It's a lot faster to run this at the database engine level, and merely return the single integer value, than to fetch all matching records and then do the count in the CLR process.

## Example

```csharp
var customerCount = Database.GetList<Customer>(s => s.PurchaseCount > someValue ).Count();
```

*should be* 🡻

```csharp
var customerCount = Database.Count<Customer>(s => s.PurchaseCount > someValue );
```
