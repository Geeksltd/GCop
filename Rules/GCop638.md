# GCop638

> *"Shorten this method by defining it as expression-bodied."*


## Rule description
expression-body are used when you have a relatively short, often frequently repeated methods.
It provides better readability and saves some space in your code.

## Example 1
```csharp
public bool Check()
{
    return boolValue;
}
```
*should be* 🡻

```csharp
public bool Check() => boolValue;
```

