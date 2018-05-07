# GCop 626

> *"The condition is unnecessary."*

## Rule description
A call to `Append(...)` method of a `StringBuilder` object will be ignored if the parameter is `null` or `empty string`. Therefore it's unnecessary to add such logic in your code.

## Example

```csharp
if (myString.HasValue())
    myStringBuilder.Append(myString);
```

*should be* 🡻

```csharp
myStringBuilder.Append(myString);
```
