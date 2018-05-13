# GCop 434

> *"Class name is unnecessary here. Static members can be called directly."*

## Rule description

You can access a static member in the same class by simply specifying its name directly.

## Example

```csharp
public class MyClass
{
    public int MyProperty
    {
        get { return Settings.MyMethod(); }
        set { MyProperty = value; }
    }  
  
    static void MyMethod()
    {
        ...
    }
}
```

*should be* 🡻

```csharp
public class MyClass
{
    public int MyProperty
    {
        get { return MyMethod(); }
        set { MyProperty = value; }
    }  
  
    static void MyMethod()
    {
        ...
    }
}
```