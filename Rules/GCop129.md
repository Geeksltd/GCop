# GCop129

> *"Change to an instance method, instead of taking a parameter  with the same type as the class."*


## Rule description
The decision to use either instance or static methods is not influenced by performance reasons, but by how the method is intended to be used. 
If the method relates to a class primary concern, put it in an instance method.


## Example 1
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

