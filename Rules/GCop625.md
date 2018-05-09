# GCop 625

> *"Using `!=` operator on the entity is not converted into SQL. Use x.ID != [...].ID instead."*

## Rule description

Since Entity Framework needs to translate your LINQ statements to SQL statements and `!=` operator is not converted into SQL, you should compare the properties of your objects.

## Example

```csharp
Database.GetList<Customer>(c => c != myCustomer);
```

*should be* 🡻

```csharp
Database.GetList<Customer>(c => c.ID != myCustomer.ID);
```