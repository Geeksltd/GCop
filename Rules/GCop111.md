# GCop 111

> *"Use implicit generic method typing"*

## Rule description

When C# can infer the generic type of a method, it's cleaner to remove explicit type from the call.

## Example

```csharp
Database.Save<Foo>(foo);
```

*should be* 🡻

```csharp
Database.Save(foo);

```
