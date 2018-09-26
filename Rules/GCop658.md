# GCop 658

> *"Use compound assignment."*

## Rule description

Compound assignment operators provide two benefits. Firstly, they produce more compact code; they are often called shorthand operators for this reason. Secondly, the variable being operated upon, or operand, will only be evaluated once in the compiled application. This can make the code more efficient.

## Example

```csharp
someInteger = someInteger + 2;
```

*should be* 🡻

```csharp
someInteger += 2;
```

