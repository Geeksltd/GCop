# GCop 306

> *"Instead use \{`<` Or `>` Or `<=` Or `>=` Or `> and <` } so it can be converted into a SQL statement and run faster"*

## Rule description

...

## Example

```csharp
Database.GetList<LogonFailure>(f => f.Date.IsBefore(someDate));
```

*should be* 🡻

```csharp
Database.GetList<LogonFailure>(f => f.Date < someDate);
```