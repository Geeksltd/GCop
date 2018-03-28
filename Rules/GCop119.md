# GCop119

> *"Don’t use **ref / out** parameters in method definition. To return several objects, define a class or struct for your method return type."*


## Rule description
OUT and REF keywords are used to return more than one value from a method. It's usually a bad idea because:
- If the return values are related to each other, they perhaps represent a missing abstraction. So you should create a class or struct to properly name that abstraction and just return an instance of that type.
- If the two values are not related, then the method is trying to do too much.

There are rare cases, mainly for performance optimization, where this rule can be ignored. The .NET framework e.g. uses it for parsing string objects, or for searching through dictionaries.

## Example 1
```csharp
void ReSize(ref int width, ref int height)
{
    ...
}
```
*should be* 🡻

```csharp
Size ReSize(int width, int height)
{
    ...    
}

public class Size
{
    public int Width {get; set;}
    public int Height {get; set;}
}
```

