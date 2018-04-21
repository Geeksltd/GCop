# GCop 165

> *"Instead of .IndexOf(\{intendedChar}) > -1 use .Contains(\{intendedChar})."*

## Rule description

`String.Contains()` Returns a value indicating whether a specified substring occurs within this string. `String.Contains()` is more faster than `String.IndexOf` and also is more readable if you don't need the index.

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