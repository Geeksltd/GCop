# GCop 506

> *"Call string.Concat instead of string.Join"*

## Rule description

The implementations of `string.Concat` and `string.Join` in .NET 4.0 are the same except `string.Join` repeatedly appends the separator. Appending an empty string is fast, but not doing so is even faster, so the `string.Concat` method would be superior here.

## Example

```csharp
string someString = string.Join("", "a", "b", "c");
```

*should be* 🡻

```csharp
string someString = string.Concat("a", "b", "c");
```
