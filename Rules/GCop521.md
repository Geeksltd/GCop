# GCop 521

> *"Change it to {'NotRoundedObject'}.Round(digits)."*

## Rule description

The `Round()` extension method on decimal and double types reads better and should be used instead of `Math.Round()`.

## Example

```csharp
var roundedVar = Math.Round(discount,0);
```

*should be* 🡻

```csharp
var roundedVar = discount.Round(0);
```
