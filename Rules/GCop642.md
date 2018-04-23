# GCop 642

> *"Replace with \{whenTrueReturnType}.Or(\{whenFalseReturnType})"*

## Rule description

The `string.Or(string)` method, gets the same string if it is not null or empty. Otherwise it returns the specified default value. It is more readable and meaningful rather than conditional `HasValue()` expression.

## Example

```csharp
var result = myString.HasValue() ? myString : "Anotherthing";
```

*should be* 🡻

```csharp
var result = myString.Or("Anotherthing");
```