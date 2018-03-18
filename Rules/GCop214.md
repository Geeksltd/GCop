# GCop214

> *"The variable defined to return the result of the method should be named **result**"*


## Rule description
Readability will increase if the variable is named result. This makes your intention clear and it is descriptive of the role of the variable.

## Example 1
```csharp
public static string getme()
{
    var res = "something";
    res = "another-thing";
    return res;
}
```
*should be* 🡻

```csharp
public static string getme()
{
    var result = "something";
    result = "another-thing";
    return result;
}
```

