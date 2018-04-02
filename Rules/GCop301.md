# GCop301

> *"Do not throw exceptions using default constructor or with empty message"*


## Rule description
An exception with no error message is confusing and hard to debug.

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
