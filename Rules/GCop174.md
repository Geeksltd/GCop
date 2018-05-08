# GCop 174

> *"You should use the method `Exists()` instead of the property, because the property caches the result - which can cause problems."*

## Rule description

In `FileInfo` and `DirectoryInfo` objects there is a property named `Exists`. But it caches the value upon first call. If after calling it once, the file existence situation changes, it will not reflect that. 

To avoid that problem use the `.Exists()` extension method instead.

## Example

```csharp
if (myFileInfo.Exists)
{
    ...
}
```

*should be* 🡻

```csharp
if (myFileInfo.Exists())
{
    ...
}
```
