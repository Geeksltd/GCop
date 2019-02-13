# GCop 420

> *"Methods should not be empty."*
>
> *"Methods should not be empty. If it's only for "IFoo" interface compliance, use explicit interface method implementation."*
> 
> *"Constructor should not be empty."*

## Rule description

Empty methods not having any nested comments are just tolerated in `abstract` classes as those empty methods are usual when implementing the visitor pattern.

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
public class Foo : IDisposable
{
    ...
    public void Dispose()
    {
    }
}
```
*should be* 🡻

```csharp
public class Foo : IDisposable
{
    ...
    void IDisposable.Dispose() { }
}
```

## Example3

```csharp
public class Foo 
{
    public Foo()
    {
    }
    ...
}
```
*should be* 🡻

```csharp
public class Foo
{
    ...
}
```
