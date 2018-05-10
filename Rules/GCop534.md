# GCop 534

> *"Use yourMemberInfo.Defines< TYPE > instead."*

## Rule description
This extension method provides a more readable and clear alternative for a common reflection requirement, i.e. to check if a type, property, field or method defines an attribute.

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
