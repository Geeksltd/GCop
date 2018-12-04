# GCop 618 (OBSOLETE)

> *"Use `{EntityName.FindByPropertyName("SomeValue")}` instead."*

## Rule description

M# generates a method for each property which is marked as unique, to find a record in the database based on the property value. this method is FindByPropertyName, where PropertyName is the name of your unique property.

## Example

```csharp
var result = MSharp.Framework.Database.Find<User>(u=> u.Email == "SomeEmail");
```

*should be* 🡻

```csharp
var result = User.FindByEmail("SomeEmail");
```
