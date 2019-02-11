# GCop 413

> *"It should be written as 'PositiveExpression'"*

## Rule description

Human brain can understand positive expressions and statements faster than negative ones.

## Example

```csharp
var bar = !(foo < bar);
```

*should be* 🡻

```csharp
var bar = foo >= bar;
```
