# GCop 430

> *"Use \" NotEqualsExpression.HasValue \" as negative logic is taxing on the brain."*

## Rule description

Human brain can understand positive expressions and statements faster than negative ones. To improve code readability its better to use *HasValue* rather than * != null *. 

## Example

```csharp
if (myVariable != null)
{
     ...
}
```

*should be* 🡻

```csharp
if (myVariable.HasValue)
{
    ...
}
```
