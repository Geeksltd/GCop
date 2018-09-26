# GCop 322

> *"Call `Enumerable.Skip` and `Enumerable.Any` instead of `Enumerable.Count`"*

## Rule description

`Any()` will generally be quicker, as it only has to look at one iteration,it returns true as soon as first element matching the condition is found. Whereas `Count()` has to go till the end of the collection to get its result.

## Example

```csharp
if (myEnumerable.Count() > someValue)
{
    ...
}
```

*should be* 🡻

```csharp
if (myEnumerable.Skip(someValue).Any())
{
    ...
}
```

