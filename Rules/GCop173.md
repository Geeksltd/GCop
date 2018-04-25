# GCop 173

> *"Don't throw exception inside static constructors."*

## Rule description

If any exception is thrown from within a static constructor, a `TypeInitializationException` will be thrown, with an `InnerException` set to the original exception that occurred. At that point, you can no longer access any static data or methods in the class. You also can’t create any objects of the type using instance constructors. After the exception, the type is unusable. It can be very hard to debug.

To avoid such problems, if your initializatoin code can throw exceptions (either explicitly or by calling other things) then it's best to avoid writing that in the static constructor. Instead, create an `Initialize()` method that is called explicitly by the consumer, so it can fall in the standard exception handling flow.

## Example

```csharp
public class MyClass
{
    ...
    static MyClass()
    {
        ...
        if (...)
           throw new Exception("SomeText");
    }
}
```

*should be* 🡻

```csharp
public class MyClass
{
    static bool IsInitialized;
    ...
    public static void Initialize()
    {
        ...
        if (...)
           throw new Exception("SomeText");
        else IsInitialized = true;
    }
    
    void AnyOtherMethodThatReliesOnInitialization()
    {
        if (!IsInitialized) 
            throw new InvalidOperationException("MyClass.Initialize() is not called...");
        
        ...
    }
}
```
