# GCop 661

> *"Simplify `if` statement."*

## Rule description

The far less confusing design for empty `if` block, is to just NOT (`!`) the condition and then have an if with no else.

## Example

```csharp
if (Condition)
{
}
else
{
  SampleMethod();
}
```

*should be* 🡻

```csharp
if (!Condition)
{
  SampleMethod();
}
```

