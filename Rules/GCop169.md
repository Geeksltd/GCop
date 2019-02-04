# GCop 169

> *"Remove `foo.HasValue` which is redundant. If `foo.HasValue` is null, then 'rightExpression' will be false anyway."*

## Rule description

When you perform comparisons with nullable types, if the value of one of the nullable types is `null` and the other is not, all comparisons evaluate to false and there is no need to check with `HasValue` again.
## Example

```csharp
if (foo.HasValue && foo.Value == decimal.Zero)
{
    ...
}
```

*should be* 🡻

```csharp
if (foo == decimal.Zero)
{
    ...
}
```