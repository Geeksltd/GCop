# GCop 169

> *"Remove \{NullableObject.HasValue} which is redundant. If \{NullableObject.HasValue} is null, then '\{rightExpression}' will be false anyway."*

## Rule description

When you perform comparisons with nullable types, if the value of one of the nullable types is `null` and the other is not, all comparisons evaluate to false and there is no need to check with `HasValue` again.
## Example

```csharp
if (myNullableDecimal.HasValue && myNullableDecimal.Value == decimal.Zero)
{
    ...
}
```

*should be* 🡻

```csharp
if (myNullableDecimal == decimal.Zero)
{
    ...
}
```