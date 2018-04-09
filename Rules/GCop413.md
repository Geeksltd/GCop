# GCop 413

> *"It should be written as 'positive expression'"*

## Rule description

Human brain can understand positive expressions and statements faster than negative ones.

## Example

```csharp
var myVar = !(something < otherthing);
```

*should be* 🡻

```csharp
var myVar = something > otherthing;
```
