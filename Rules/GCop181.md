# GCop 181

> *"Define a virtual method and write this logic using polymorphism."*

## Rule description

A base class should not know about its subclasses. When your logic depends on the actual type of the object, you need to use virtual methods because your program may be designed in such a way that you do not know all the types of objects that will occur when it is executed. You can provide a standard (base) type and design around that type.

## Example

```csharp
public class BaseClass
{
    ...
    public void SomeMethod()
    {
        if (this is DerivedClass)
        {
            // Implementation A
        }
        else
        {
            // Implementation B
        }
    }
}
```

*should be* 🡻

```csharp
public class BaseClass
{
    ...
    public virtual void SomeMethod()
    {
        // Implementation B
    }
}

public class DerivedClass : BaseClass
{
    public override void SomeMethod()
    {
        // Implementation A
    }
}
```
