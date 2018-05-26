# GCop 434

> *"Class name is unnecessary here. Static members can be called directly."*

## Rule description
> This seems wrong. See https://github.com/Geeksltd/GCop/issues/155

You can access a static member in the same class by simply specifying its name directly.

## Example1

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

## Example2

```csharp
public class MyClass
{
    public int MyProperty
    {
        get { return MyClass.MyMethod(); }
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
