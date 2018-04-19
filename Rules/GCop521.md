# GCop 521

> *"Change it to {'NotRoundedObject'}.Round(digits)."*

## Rule description

...

## Example

```csharp
var roundedVar = Math.Round(discount,0);
```

*should be* 🡻

```csharp
var roundedVar = discount.Round(0);
```