# GCop 420

> *"Methods should not be empty."*
>
> *"Methods should not be empty. If it's only for "interfaceName" interface compliance, use explicit interface method implementation."*

## Rule description

Empty methods not having any nested comments are just tolerated in Abstract classes as those empty methods are usual when implementing the visitor pattern.

## Example1

```csharp
public static void Log(this Exception ex)
{
}
```

*should be* 🡻

```csharp
public static void Log(this Exception ex)
{
    // some code ...
}
```

## Example2

```csharp
public class MyClass : IHttpModule
{
    ...
    public void Dispose()
    {
    }
}
```
*should be* 🡻

```csharp
public class MyClass : IHttpModule
{
    ...
    void IHttpModule.Dispose() { }
}
```
