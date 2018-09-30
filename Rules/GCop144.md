# GCop 144

> *"Since there is only one statement, remove the unnecessary braces and write it as `Database.Update( myObject, x => x.Abc = value );`"*

## Rule description

Actions braces (`{ ... }`) should be removed when there is only one statement. Adding the braces will be unnecessary noise.

## Example

```csharp
Database.Update(foo, u => { foo.Bar = "something"; });
```

*should be* 🡻

```csharp
Database.Update(foo, u => foo.Bar = "something");
```