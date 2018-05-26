# GCop 510

> *"`None()` already handles the null scenario. Remove unnecessary part: `{ColectionName} == null`."*

## Rule description

`None()` is an MSharp extension which determines if collection is null or empty and return a `boolean` value. So there is no need to handle null scenario while using this method.

## Example

```csharp
if (someCollection == null || someCollection.None())
{
    ...
}
```

*should be* 🡻

```csharp
if (someCollection.None())
{
    ...
}
```