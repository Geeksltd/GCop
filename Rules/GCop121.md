# GCop 121

> *"Use **numeric string.To< data type >()** instead of **data type.Parse(numeric string)**"*

## Rule description

The To<...>() extension method on the string type allows you to make type conversions in a uniform way for many types. It's also briefer and more readable. Just like you can say myInt.To**String**(), you can say myString.To<**int**>(). 

## Example

```csharp
public long Calc(string commision)
{
    var calculatedCommision = long.parse(commision);
    ...
    return calculatedCommision;
}
```

*should be* 🡻

```csharp
public string Calc(string commision)
{
    var calculatedCommision = commision.To<long>();
    ...
    return calculatedCommision;
}
```