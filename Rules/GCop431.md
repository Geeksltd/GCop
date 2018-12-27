# GCop 431

> *"Use `foo is Bar` as negative logic is taxing on the brain."*

## Rule description

Human brain can understand positive expressions and statements faster than negative ones. To improve code readability it is better to use `if(expression is type)` rather than `if(expression as type ! = null)`.

## Example

```csharp
if(foo as string != null)
{
    ...
}
```

*should be* 🡻

```csharp
if(foo is string)
{
    ...
}
```