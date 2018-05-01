# GCop 111

> *"Use implicit generic method typing"*

## Rule description

The M# offers `Databas.Save()` method to persist an entity instance in the database.

## Example

```csharp
Database.Save<myEntityName>(myEntityInstance);
```

*should be* 🡻

```csharp
Database.Save(myEntityInstance);

```