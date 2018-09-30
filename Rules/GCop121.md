# GCop 121

> *"Use `numericString.To<DataType>()` instead of `DataType.Parse(numericString)`"*

## Rule description

The `To<...>()` extension method on the string type allows you to make type conversions in a uniform way for many types. It's also briefer and more readable. Just like you can say myInt.To**String**(), you can say myString.To<**int**>(). 

## Example

```csharp
public long Foo(string commision)
{
    return long.Parse(commision);
}
```

*should be* 🡻

```csharp
public string Foo(string commision)
{
    return commision.To<long>();
}
```