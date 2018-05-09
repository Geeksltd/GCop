# GCop 613

> *"It should be written as `Database.Get<{T}>({Guid})`."*

## Rule description

The `.Get()` method should be used when single record is required from database based on the `ID` of the entity. This method also boosts performance because M# runs the query in SQL environment. It throws `System.ArgumentNullException`, if the input parameter is null or empty.

## Example

```csharp
Database.Find<User>(s => s.ID == myGuid);
```

*should be* 🡻

```csharp
Database.Get<User>(myGuid);
```