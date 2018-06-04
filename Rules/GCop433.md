# GCop 433

> *"Class name is unnecessary here. Static members can be called directly."*

## Rule description

You can access a static member in the same class by simply specifying its name directly.

## Example

```csharp
public class MyClass
{
    ...
    static void MyMethod()
    {
        ...
        MyClass.AnotherMethod();
    }
    
    static void AnotherMethod()
    {
        ...
    }
}
```

*should be* 🡻

```csharp
public class MyClass
{
    ...
    static void MyMethod()
    {
        ...
        AnotherMethod();
    }
    
    static void AnotherMethod()
    {
        ...
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

