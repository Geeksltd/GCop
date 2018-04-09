# GCop 129

> *"Change to an instance method, instead of taking a parameter  with the same type as the class."*

## Rule description

A static method defined in a class that takes an instance of the same class is usually an indication that the responsibility (method) belongs to the instance as opposed to the class.

This is of course not always true. One exception is when the method is supposed to do something when the argument is null.
If you have made an informed decision that the method should remain static, add a [EscapeGCop] attribute and explain the reason why.

## Example

```csharp
public class MyClass
{
    public static bool Add(MyClass obj)
    {
        DbContext.EntityName.Add(obj);
        ...
    }
}
```

*should be* 🡻

```csharp
public class MyClass
{
    public static bool Add()
    {
        DbContext.EntityName.Add(this);
        ...
    }
}
```