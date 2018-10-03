# GCop 147

> *"Instead of comparing the `Id` properties, just compare the objects directly."*

## Rule description

The database engine in the M#/Olive framework will automatically convert direct object comparisons to be based on `ID` comparison. You don't need to compare the `IDs` manually. In fact, a direct comparison will result in a more efficient SQL query that avoids an unnecessary join.

## Example

```csharp
var result = Database.Find<Employee>(e => e.Company.ID == myCompany.ID );
```

*should be* 🡻

```csharp
var result = Database.Find<Employee>(e => e.Company == myCompany );
```
