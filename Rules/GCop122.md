# GCop 122

> *"Use Database.CreateTransactionScope() instead of \{TransactionScope}"*

## Rule description

...

## Example

```csharp
using (var myScope = new TransactionScope())
{
    ...
}
```

*should be* 🡻

```csharp
using (var myScope = new Database().CreateTransactionScope())
{
    ...
}
```