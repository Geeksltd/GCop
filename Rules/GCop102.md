# GCop 102

> *"Use \{"Address"}.AsDirectory() instead of \{new DirectoryInfo("Address")}"*

## Rule description

The `AsDirectory()` converts the path into a directory object. It is more readable and easy to understand rather than `DirectoryInfo()`.

## Example

```csharp
var myDirectory = new DirectoryInfo(@"c:\MyAddress");
```

*should be* 🡻

```csharp
var myDirectory = @"c:\MyAddress".AsDirectory();
```
