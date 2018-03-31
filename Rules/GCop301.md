# GCop301

> *"Do not throw exceptions using default constructor or with empty message"*


## Rule description
Throw exceptions using default constructor creates a brand new Exception instance, losing the original stack trace of the exception, as well as its type. (eg, IOException).
In addition, some exceptions hold additional information (eg, ArgumentException.ParamName).
throw new Exception(ex.Message); will destroy this information too.

## Example 1
```csharp
public static void MyMethod()
{   
    if(something == anotherthing)
    {
        throw new Exception();
    }   
}
```
*should be* 🡻

```csharp
public static void MyMethod()
{   
    if(something == anotherthing)
    {
        throw new Exception("some error message");
    }   
}
```
