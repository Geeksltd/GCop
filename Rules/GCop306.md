# GCop 306

> *"Instead use \{`<` Or `>` Or `<=` Or `>=` Or `> and <` } so it can be converted into a SQL statement and run faster"*

## Rule description

Some extension methods on `DateTime` such as `IsBefore(...)` and `IsBetween(...)` are not recognised by the LINQ to SQL convertor in the database engine. So you should write the simpler alternatives using standard operators instead.

## Example

```csharp
Database.GetList<LogonFailure>(f => f.Date.IsBefore(someDate));
```

*should be* 🡻

```csharp
Database.GetList<LogonFailure>(f => f.Date < someDate);
```
