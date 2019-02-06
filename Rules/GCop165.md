# GCop 165

> *"Instead of `.IndexOf(foo) > -1` use `.Contains(foo)`."*

## Rule description

The `String.Contains()` method returns a `bool` value indicating whether a specified substring occurs within this string. `String.Contains()` is more expressive and readable than `String.IndexOf()`.

## Example

```csharp
if (myString.IndexOf("?") > -1)
{
    ...
}
```

*should be* 🡻

```csharp
if(myString.Contains("?"))
{
    ...
}
```
