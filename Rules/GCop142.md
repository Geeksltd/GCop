# GCop 142

> *"Replace `!string.HasValue()` with `string.IsEmpty()`"*
> 
> *"Replace `!string.IsEmpty()` with `string.HasValue()`"*

## Rule description

Human brain can understand positive expressions and statements faster than negative ones. To improve code readability it is better to use `HasValue` rather than `!IsEmpty`, and `IsEmpty` rather than `!HasValue`.

## Example1

```csharp
if (!bar.HasValue())
{
    ...
}
```

*should be* 🡻

```csharp
if (bar.IsEmpty())
{
    ...
}
```

## Example2

```csharp
if (!bar.IsEmpty())
{
    ...
}
```

*should be* 🡻

```csharp
if (bar.HasValue())
{
    ...
}
```