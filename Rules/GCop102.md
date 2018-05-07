# GCop 102

> *"Use \{"Address"}.AsDirectory() instead of \{new DirectoryInfo("Address")}"*

## Rule description

The `x.AsDirectory()` converts the path into a directory object as a fluent shortcut to `new DirectoryInfo(x)`.

## Example

```csharp
var myDirectory = new DirectoryInfo(somePathString);
```

*should be* 🡻

```csharp
var myDirectory = somePathString.AsDirectory();
```
