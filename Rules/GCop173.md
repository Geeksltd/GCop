# GCop 173

> *"Don't throw exception inside static constructors."*

## Rule description

If any exception is thrown from within a static constructor, a `TypeInitializationException` will be thrown, with an `InnerException` set to the original exception that occurred.  At that point, you can no longer access any static data or methods in the class.  You also can’t create any objects of the type using instance constructors.  After the exception, the type is unusable.

To fix this error, you can add `try-catch` blocks around the body of the static constructor that throws it.
## Example

```csharp
public class MyClass
{
    ...
    static MyClass()
    {
        ...
        throw new Exception("SomeText");
    }
}
```

*should be* 🡻

```csharp
public class MyClass
{
    ...
    static MyClass()
    {
        try
        {
            ...
        }   
        catch
        {
            throw new Exception("SomeText");
        }
    }
}
```