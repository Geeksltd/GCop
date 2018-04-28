# GCop 174

> *"You should use the method Exists() instead of the property because the property caches the result, which can cause problems."*

## Rule description

The `Exists()` method determines whether or not this file exists. Note that the standard `Exists` property has a caching bug, so use this for accurate result.

## Example

```csharp
var myFile = "address".AsFile();
if (myFile.Exists)
{
    ...
}
```

*should be* 🡻

```csharp
var myFile = "address".AsFile();
if (myFile.Exists())
{
    ...
}
```
