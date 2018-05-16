# GCop 181

> *"Define a virtual method and write this logic using polymorphism."*

## Rule description

You need to use virtual methods because your program may be designed in such a way that you do not know all the types of objects that will occur when it is executed. You can provide a standard (base) type and design around that type.

## Example

```csharp
public class BaseClass
{
    ...
    public void Mymethod()
    {
        ...
        if (this is DerivedClass)
            {
                ...
            }
    }
}
```

*should be* 🡻

```csharp
public class BaseClass
{
    ...
    public virtual void Mymethod()
    {
        ...
    }
}

public class DerivedClass : BaseClass
{
    public override void MyMethod()
    {
        ...
        if (this is DerivedClass)
        {
            ...
        }
        base.MyMethod();
    }

}
```