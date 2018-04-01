# GCop538

> *"File extensions can have different casing. Use case insensitive string comparison."*


## Rule description
Use ToLower() or ToUpper() to become sure about insensitive string comparison.

## Example 1
```csharp
if ("~/Extensions/Languages.xml".AsFile() == null)
{
    ...
}
```
*should be* 🡻

```csharp
if ("~/Extensions/Languages.xml".ToLower().AsFile() == null)
{
    ...
}
```
