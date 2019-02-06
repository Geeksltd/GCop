# GCop 184

> *"Avoid using `null` on the left side of comparision statement."*

## Rule description

Using an exemplary value like `null` as the first (left) operand is counterintuitive. Instead, the first operand should suggest what you intend to evaluate. In this way the code readability will be improved.

## Example

```csharp
bool? foo;
...
if (null == foo)
{
    ...
}
```

*should be* 🡻

```csharp
bool? foo;
...
if (foo == null)
{
    ...
}
```

