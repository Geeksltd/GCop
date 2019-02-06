# GCop 301

> *"Do not throw exceptions using default constructor or with empty message"*

## Rule description

An exception with no error message is confusing and hard to debug.

## Example

```csharp
public static void Foo()
{   
    ...
    if(foo == bar)
    {
        throw new Exception();
    }   
}
```

*should be* 🡻

```csharp
public static void Foo()
{   
    ...
    if(foo == bar)
    {
        throw new Exception("some error message");
    }   
}
```