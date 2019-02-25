# GCop 433

> *"Class name is unnecessary here. Static members can be called directly."*

## Rule description

You can access a static member in the same class by simply specifying its name directly.

## Example

```csharp
public class FooBar
{
    ...
    static void Foo()
    {
        ...
        FooBar.Bar();
    }
    
    static void Bar()
    {
        ...
    }
}
```

*should be* 🡻

```csharp
public class FooBar
{
    ...
    static void Foo()
    {
        ...
        Bar();
    }
    
    static void Bar()
    {
        ...
    }
}
```

## Example2

```csharp
public class FooBar
{
    public int Bar
    {
        get { return FooBar.Foo(); }
        set { Bar = value; }
    }  

    static int Foo()
    {
        ...
    }
}
```

*should be* 🡻

```csharp
public class FooBar
{
    public int Bar
    {
        get { return Foo(); }
        set { Bar = value; }
    }  

    static int Foo()
    {
        ...
    }
}
```

