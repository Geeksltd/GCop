# GCop103

> *"Instead of null, return an empty collection such as Enumerable.Empty<method.ReturnType>"*


## Rule description
You should always return an empty list instead of null.In this way you eliminate the risk of a NullReferenceException and You don't have to check for null in client code, so your code becomes shorter, more readable and easier to maintain

This rule should not be effected when method returntype is not assignable from IEnumerable.

## Example 1
```csharp
public static List<Payment> MyMethod()
{
    if()
    {
        //return List<Payment>
    }
    else
        return null;
}
```
*should be* 🡻

```csharp
public static List<Payment> MyMethod()
{
    if()
    {
        //return List<Payment>
    }
    else
        return Enumerable.Empty<Payment>;
}
```

