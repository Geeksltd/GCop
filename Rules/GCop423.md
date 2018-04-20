# GCop 423

> *"This condition was just checked on line \{lineNumber}."*

## Rule description

Duplicate conditions are noisy, make long your code and reduce the readability of your program.

## Example

```csharp
if (someBooleanExpression){...}
else{...}

if (someBooleanExpression){...}
else{...}
```

*should be* 🡻

```csharp
if (someBooleanExpression){...}
else{...}
```