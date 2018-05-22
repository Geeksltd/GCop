# GCop 537

> *"File extensions contain the leading dot character."*

## Rule description

The `Extension` property returns the `FileSystemInfo` extension, including the period (.). It checks every char from the end of the filepath till it finds a dot, then a substring is returned from the dot to the end of the filepath. So it should be compared with a string, containing the dot character at the first.

## Example

```csharp
FileInfo fInfo = new FileInfo(path);/
if(fInfo.Extension == "txt")
{
    ...
}
```

*should be* 🡻

```csharp
FileInfo fInfo = new FileInfo(path);/
if(fInfo.Extension == ".txt")
{
    ...
}
```