# GCop430
> *"Use \" NotEqualsExpression.HasValue \" as negative logic is taxing on the brain."*


## Rule description
To improve code readability its better to use HasValue because it is more meaningful. 

## Example 1
```csharp
if (myVariable != null)
    ...
```
*should be* 🡻

```csharp
if (myVariable.HasValue)
    ...
```

