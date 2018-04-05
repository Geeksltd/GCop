# GCop217

> *"Rename the method to Is...Valid"*


## Rule description
Methods which are used for validation and return boolean result without *Throw* statements should be written like 'Is...Valid' pattern, to be more meaningful.

## Example 1
```csharp
public bool ValidateOrder()
{
    ...
}
```
*should be* 🡻

```csharp
public bool IsOrderValid()
{
    ...
}
```