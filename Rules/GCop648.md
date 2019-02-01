# GCop 648

> *"Use `foo.Remove("bar")` instead"*

## Rule description

The `Remove()` extension method on string is more readable, more explicit and briefer than using `Replace(..., "")` when you just want to eliminate a phrase.

## Example

```csharp
var result = foo.Replace("bar", "");
```

*should be* 🡻

```csharp
var result = foo.Remove("bar");
```