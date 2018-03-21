# GCop121

> *"Use StringVariable.To< {int/long} >() instead of {int/long}.Parse(StringVariable)"*


## Rule description
...

## Example 1
```csharp
public long Calc(string commision)
{
    var calculatedCommision = long.parse(commision);
    //several lines of code
    return calculatedCommision;
}
```
*should be* 🡻

```csharp
public string Calc(string commision)
{
    var calculatedCommision = commision.To<long>();
    //several lines of code
    return calculatedCommision;
}
```

