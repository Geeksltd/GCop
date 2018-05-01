# GCop 626

> *"The condition is unnecessary."*

## Rule description

The `Append` method add a copy of the specified string to this instance. If you append a null value to the `StringBuilder` object, it would be stored as a “null” (four character string) in the object instead of no value at all. So there is no need to check for `null` values, because no exception will be thrown.

## Example

```csharp
if (myString.HasValue())
    myStringBuilder.Append(myString);
```

*should be* 🡻

```csharp
myStringBuilder.Append(myString);
```

*Or* 🡻

```csharp
myStringBuilder.Append(myString ?? "");
```