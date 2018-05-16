# GCop 429

> *"Change to `{LambdaIdentifier.Id} == {EntityObject}` as you can compare Guid with Entity directly. It handles null too."*

## Rule description

Science you can compare `Guid` with `Entity`, there is no need to compare it with `Guid` property of the other `Entity`. It handles null too.

## Example

```csharp
var result = Database.GetList<Customer>(s => s.Id == myCustomer.ID);
```

*should be* 🡻

```csharp
var result = Database.GetList<Customer>(s => s.Id == myCustomer);
```