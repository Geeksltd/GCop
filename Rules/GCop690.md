# GCop 690

> *"Negative logic is taxing on the brain. Use variable == null instead."*

## Rule description

Human brain can understand positive expressions and statements faster than negative ones. To improve code readability its better to check for null rather than negative HasValue. 

## Example

```csharp
public bool Check(int? myVar)
{
    if (!myVar.HasValue) return false;
    ...
    return true;
}
```

*should be* 🡻

```csharp
public bool Check(int? myVar)
{
    if (myVar == null) return false;
    ...
    return true;
}
```