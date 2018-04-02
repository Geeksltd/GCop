# GCop638

> *"Shorten this method by defining it as expression-bodied."*


## Rule description
Short methods that can be written in a single line should be written as expression-bodied which is a briefer and more compact format, allowing readers to see more on the screen.

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

