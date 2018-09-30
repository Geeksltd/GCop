# GCop 122

> *"Use `Database.CreateTransactionScope()` instead of `TransactionScope`"*

## Rule description

In M# and Olive applications, if you need a transaction scope, you should use the transaction scope factory method from the Database class rather than creating a new transaction scope manually. The reason is that each application can potentially define a different transaction implementation mechanism, which should be used consistently throughout the application.

## Example

```csharp
using (var myScope = new TransactionScope())
{
    ...
}
```

*in M# apps should be* 🡻

```csharp
using (var myScope = Database.CreateTransactionScope())
{
    ...
}
```

*in Olive apps should be* 🡻

```csharp
using (var myScope = Context.Current.Database().CreateTransactionScope())
{
    ...
}
```