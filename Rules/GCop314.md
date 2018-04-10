# GCop 314

> *"You don't need the Where clause. Replace with FirstOrDefault(LambdaExpression)"*

## Rule description

Using Where with `FirstOrDefault` clause or using `FistOrDefault` alone will make the same SQL statement. While the second approach is shorter and more meaningful.

## Example

```csharp
var myObj = db.EntityName.Where(a => a.Id == id).FirstOrDefault();
```

*should be* 🡻

```csharp
var myObj = db.EntityName.FirstOrDefault(a => a.Id == id);
```
