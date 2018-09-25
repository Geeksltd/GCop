# GCop 645

> *"Use ^ operator"*

## Rule description

Binary `^` operators are predefined for the integral types and `bool`. For integral types, `^` computes the bitwise exclusive-OR of its operands. For `bool` operands, `^` computes the logical exclusive-or of its operands; that is, the result is `true` if and only if exactly one of its operands is `true`. So to simplify an if condition like the below example, use `^` operator, which result is exactly the same.

## Example

```csharp
if ((someBoolVar && !anotherBoolVar) || (!someBoolVar && anotherBoolVar))
{
    ...
}
```

*should be* 🡻

```csharp
if (someBoolVar ^ anotherBoolVar)
{
    ...
}
```
