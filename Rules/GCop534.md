# GCop 534

> *"Use yourMemberInfo.Defines< TYPE > instead."*

## Rule description
This extension method provides a more readable and clear alternative.

## Example

```csharp
if (Attribute.IsDefined(myElement, typeof(EnableLogAttribute)))
{
   ...
}
```

*should be* 🡻

```csharp
if (myElement.Defines<EnableLogAttribute>())
{
   ...
}
```
