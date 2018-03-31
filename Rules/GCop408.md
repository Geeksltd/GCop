# GCop408

> *"Boolean parameters should go after all non-optional parameters."*


## Rule description
It is just a style to have a more regular code.

## Example 1
```csharp
public static void MyMethod(bool firstParam, string secondParam)
{
    ...
}
```
*should be* 🡻

```csharp
public static void MyMethod(string secondParam, bool firstParam)
{
    ...
}
```

