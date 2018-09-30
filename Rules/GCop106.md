# GCop 106

> *"Use `path.AsFile()` instead of `New FileInfo(path)`"*

## Rule description

using `AsFile()` makes your code more meaningful and shorter. Also it increase readability.

## Example

```csharp
var file = new FileInfo(address);
```

*should be* 🡻

```csharp
var file = address.AsFile();
```