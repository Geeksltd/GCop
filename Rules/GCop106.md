# GCop 106

> *"Use **FileAddressString.AsFile()** instead of New **FileInfo(FileAddressString)**"*

## Rule description

using `AsFile()` makes your code more meaningful and shorter. Also it increase readability.

## Example

```csharp
var file = new FileInfo(HttpContext.Current.Server.MapPath("FileAddress"));
```

*should be* 🡻

```csharp
var file = "FileAddress".AsFile();
```