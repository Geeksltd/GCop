# GCop112

> *"This class is too large. Break its responsibilities down into more classes."*


## Rule description
When classes get too big, it is likely they have too many responsibilities and it is against SOLID first rule, which stands for "Single responsibility". 
Define two or more responsibilities within the boundaries of what the class is doing, separate theme into two, or more, classes.

## Example 1
```csharp
public class MyClass
{
    //More than 1000 lines
}
```
*should be* 🡻

```csharp
public class MyClass
{
    //This GodClass uses all the new classes, exposes the same methods to the rest of the system, but does not implement any functionality in itself.
}
```

