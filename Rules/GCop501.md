# GCop 501

> *"Do not call {'MethodName'} method of class {'ClassName'} as a static method"*

## Rule description

The language design force us to call the extension method like an instance method, but is actually a static method.

## Example

```csharp
public static class Util
{
    ...
    public static void Log(this string message, string fileName)
    {
        ...
    }
}
public class AnotherClass
{
    ...
    public static void MyMethod()
    {
        ...
        Util.Log(message, "fileName");             
    }
}
```

*should be* 🡻

```csharp
public static class Util
{
    ...
    public static void Log(this string message, string fileName)
    {
        ...
    }
}
public class AnotherClass
{
    ...
    public static void MyMethod()
    {
        ...
        message.Log("fileName");             
    }
}
```