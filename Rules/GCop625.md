# GCop 625

> *"Using `!=` operator on the entity is not converted into SQL. Use x.ID != [...].ID instead."*

## Rule description

The Linq to SQL converter in M# Framework / Olive requires this to work correctly.

## Example

```csharp
Database.GetList<Customer>(c => c != myCustomer);
```

*should be* 🡻

```csharp
Database.GetList<Customer>(c => c.ID != myCustomer.ID);
```
