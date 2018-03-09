# GCop103

> *"Instead of null, return an empty collection such as Enumerable.Empty<method.ReturnType>"*


## Rule description
If your method returns an enumerable, then you should always return an empty list instead of null. In this way you eliminate the risk of a NullReferenceException and You don't have to check for null in client code, so your code becomes shorter, more readable and easier to maintain. [Further reading](https://orcharddojo.net/orchard-resources/Library/DevelopmentGuidelines/BestPractices/CSharp).



## Example 1
```csharp
public static IEnumerable<Payment> MyMethod()
{
    if()
    {
        //return IEnumerable<Payment>
    }
    else
        return null;
}
```
*should be* 🡻

```csharp
public static IEnumerable<Payment> MyMethod()
{
    if()
    {
        //return IEnumerable<Payment>
    }
    else
        return Enumerable.Empty<Payment>;
}
```

