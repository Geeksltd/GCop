# GCop119

> *"Don’t use **ref / out** parameters in method definition. To return several objects, define a class or struct for your method return type."*


## Rule description
You almost never need to use ref/out. It's basically a way of getting another return value, and should usually be avoided precisely because it means the method's probably trying to do too much. That's not always the case (TryParse etc are the canonical examples of reasonable use of out) but using ref/out should be a relative rarity.

## Example 1
```csharp
private void ReSize(ref int width, ref int height)
{
    ...
}
```
*should be* 🡻

```csharp
private CustomizedSize ReSize(int width, int height)
{
    ...
    return CustomizedSizeObject;
}
public class CustomizedSize
{
    int width {get; set;}
    int height {get; set;}
}
```

