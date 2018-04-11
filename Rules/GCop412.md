# GCop 412

> *"Don't hardcode a path. Consider using “AppDomain.CurrentDomain.GetPath()" instead."*

## Rule description

...

## Example

```csharp
File.AppendAllText(@"C:\PathString", "ContentString");
```

*should be* 🡻

```csharp
File.AppendAllText(AppDomain.CurrentDomain.Getpath("C:\PathString"), "ContentString");
```
