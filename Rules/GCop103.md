# GCop 103

> *"Instead of `null`, return an empty collection such as `Enumerable.Empty<methodReturnType>`"*

## Rule description
If your method returns an enumerable, then you should always return an empty list instead of `null`. In this way you eliminate the risk of a `NullReferenceException` and You don't have to check for `null` in client code, so your code becomes shorter, more readable and easier to maintain.

For example, if a user tries to use it with `foreach` or `Linq`, it won't crash, but will just "skip" the loop (since there are no elements). [Further reading](https://orcharddojo.net/orchard-resources/Library/DevelopmentGuidelines/BestPractices/CSharp).

## Example

```csharp
public static IEnumerable<Bar> Foo()
{
    if(condition)
    {
        //return IEnumerable<Bar>
    }
    else
        return null;
}
```

*should be* 🡻

```csharp
public static IEnumerable<Bar> Foo()
{
    if(condition)
    {
        //return IEnumerable<Bar>
    }
    else
        return Enumerable.Empty<Bar>;
}
```