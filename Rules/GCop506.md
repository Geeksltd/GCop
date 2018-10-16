# GCop 506

> *"Call `string.Concat` instead of `string.Join`"*

## Rule description

The implementations of `string.Concat` and `string.Join` in .NET 4.0 are the same except `string.Join` repeatedly appends the separator. Appending an empty string is fast, but not doing so is even faster, so the `string.Concat` method would be superior here.

## Example1

```csharp
var foo = string.Join("", "a", "b", "c");
```

*should be* 🡻

```csharp
var foo = string.Concat("a", "b", "c");
```

## Example2

```csharp
var bar = new List<string>() { "cat", "dog", "perls" };
var foo = string.Join("", bar);
```

*should be* 🡻

```csharp
var bar = new List<string>() { "cat", "dog", "perls" };
var foo = string.Concat(bar);
```