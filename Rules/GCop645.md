# GCop 645

> *"Use ^ operator"*

## Rule description

Binary `^` operators are predefined for the integral types and bool. For integral types, `^` computes the bitwise exclusive-OR of its operands. For `bool` operands, `^` computes the logical exclusive-OR of its operands; that is, the result is `true` if and only if exactly one of its operands is `true`. So to simplify an if condition like the below example, use `^`, XOR operator, which result is exactly the same.

## Example

```csharp
if ((boolFoo && !boolBar) || (!boolFoo && boolBar))
{
    ...
}
```

*should be* 🡻

```csharp
if (boolFoo ^ boolBar)
{
    ...
}
```
