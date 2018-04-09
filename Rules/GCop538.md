# GCop 538

> *"File extensions can have different casing. Use case insensitive string comparison."*

## Rule description

File extensions in Windows are treated case insensitively. So you can have a file named something.jpg or something.JPG or something.Jpg and they will all work fine. But if in your application logic you hard-code a specific casing, you can have a problem. See the below example to learn more.

## Example

```csharp
if (someFileInfo.Extension == ".xml")
{
    ...
}
```

*should be* 🡻

```csharp
if (someFileInfo.Extension.ToLower() == ".xml")
{
    ...
}
```