# GCop 434

> *"Class name is unnecessary here. Static members can be called directly."*

## Rule description

You can access a static member in the same class by simply specifying its name directly.

## Example

```csharp
public class MyClass
{
    static void MyMethod()
    {
        ...
    }
    
    void AnotherMethod()
    {
        ...
        MyClass.MyMethod(); 
    }
}
```

*should be* 🡻

```csharp
public class MyClass
{
    static void MyMethod()
    {
        ...
    }
    
    void AnotherMethod()
    {
        ...
        MyMethod(); 
    }
}
```
