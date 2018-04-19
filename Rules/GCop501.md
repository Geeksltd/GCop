# GCop 501

> *"Do not call {'MethodName'} method of class {'ClassName'} as a static method"*

## Rule description

Every extension method is a static method, and therefore can be called directly. However, that is ugly and defeats the purpose. After all, the method is defined as an *extension method* rather than a normal method so it can be called like an *instance method*.

## Example

```csharp
public static class MyBlahBlahExtensions
{
    public static string GetSomething(this string @this)
    {
        ...
    }
}

    ...
    void SomeCaller()
    {
        ...
        var x = MyBlahBlahExtensions.GetSomething(message);             
    }

```

*should be* 🡻

```csharp
public static class MyBlahBlahExtensions
{
    public static void GetSomething(this string @this)
    {
        ...
    }
}

    ...
    void SomeCaller()
    {
        ...
        var x = message.GetSomething();             
    }

```
