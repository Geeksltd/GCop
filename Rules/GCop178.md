# GCop 178

> *"Use parenthesis to clarify your boolean logic intention."*

## Rule description

Technically, the AND (`&&`) operator has higher precedence than the OR (`||`) operator.
So the expression of `a || b && c` is the same as `a || (b && c)` as opposed to `(a || b) && c`.

When you have a mix of `&&` and `||` operators, even though the parenthesis may be optional in your case, but for better clarity and lack of confusion, it's better to be explicit about it. Explicit paranthesis will not only make it easier for everyone to quickly understand your logic, but this can also prevent accidental errors when refactoring the code later on.

## Example

```csharp
if (foo > 200 && foo > 250 || bar == 500)
{
    ...
}
```

*should be* 🡻

```csharp
if (foo > 200 && (foo > 250 || bar == 500))
{
    ...
}
```
