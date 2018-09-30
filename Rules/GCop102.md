# GCop 102

> *"Use `path.AsDirectory()` instead of `new DirectoryInfo(path)`"*

## Rule description

The `x.AsDirectory()` converts the path into a directory object as a fluent shortcut to `new DirectoryInfo(x)`.

## Example

```csharp
var directory = new DirectoryInfo(address);
```

*should be* 🡻

```csharp
var directory = address.AsDirectory();
```