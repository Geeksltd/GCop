# GCop 611

> *"It should be written as `Database.Reload({ParameterName})`."*

## Rule description

...

## Example

```csharp
Database.Get<Customer>(myCustomer.ID);
```

*should be* 🡻

```csharp
Database.Reload(myCustomer);
```