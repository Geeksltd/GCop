# GCop 519

> *"Use `MaxOrNull(LambdaExpression)` instead."*
>
> *"Use `MinOrNull(LambdaExpression)` instead."*

## Rule description

The `.MaxOrNull()` and `.MinOrNull()` methods get the maximum or minimum value of the specified expression in the list. If no items exist, then null will be returned. the `.WithMax()` or `.WithMin()` method, Select the item with maximum or minimum of the specified value. So to get the max value with these one you should write more codes.

## Example

```csharp
var myResult = someCollection.WithMax(x => x.Age)?.Age;
```

*should be* 🡻

```csharp
var myResult = someCollection.MaxOrNull(x => x.Age);

```