# GCop 540

> *"`Abstract` class should not have `public` constructors. Make it `protected` instead."*

## Rule description

Being public makes no sense in an abstract class. An abstract class by definition cannot be instantiated directly. It can only be instantiated by an instance of a derived type. Therefore the only types that should have access to a constructor are its derived types and hence protected makes much more sense than public. It more accurately describes the accessibility.

## Example

```csharp
public abstract class Foo
{
    public Foo()
    {
        ...
    }
    ...
}
```

*should be* 🡻

```csharp
public abstract class Foo
{
    protected Foo()
    {
        ...
    }
    ...
}
```