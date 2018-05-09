# GCop 613

> *"It should be written as `Database.Get<{T}>({Guid})`."*

## Rule description

The `Database.Get()` method should be used when single record is required from database based on the `ID` of the entity. This method also boosts performance because M# runs the query in SQL environment, While `Database.Find()` method criteria is not always fully translated into SQL query, hence does not always run the full query in SQL server environment. So, `.Get()` method should always be called when you are sure about the passing `ID`.

## Example

```csharp
Database.Find<User>(s => s.ID == myGuid);
```

*should be* 🡻

```csharp
Database.Get<User>(myGuid);
```