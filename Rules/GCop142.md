# GCop 142

> *"Replace \{!stringObject.HasValue()} with \{stringObject.IsEmpty()}"*
> 
> *"Replace \{!stringObject.IsEmpty()} with \{stringObject.HasValue()}"*

## Rule description

Human brain can understand positive expressions and statements faster than negative ones. To improve code readability it is better to use `HasValue` rather than `!IsEmpty` and `IsEmpty` rather than `!HasValue`.

## Example1

```csharp
if (!myString.HasValue())
{
    ...
}
```

*should be* 🡻

```csharp
if (!myString.IsEmpty())
{
    ...
}
```

## Example2

```csharp
if (!myString.IsEmpty())
{
    ...
}
```

*should be* 🡻

```csharp
if (myString.HasValue())
{
    ...
}
```
