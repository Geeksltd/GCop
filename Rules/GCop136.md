# GCop136

> *"All constants and class fields should be defined at the top of the class, before all methods."*


## Rule description
According to the StyleCop Rules Documentation the ordering of items in a class is as follows: 
* Constant Fields
* Fields
* Constructors
* Finalizers (Destructors)
* Delegates
* Events
* Enums
* Interfaces
* Properties
* Indexers
* Methods
* Structs
* Classes

## Example 1
```csharp
public class MyClassName
{
    public void MyMethod() 
    {
        ...
    }
    private const string myConstVal = "someText";

}
```
*should be* 🡻

```csharp
public class MyClassName
{
    private const string myConstVal = "someText";
    
    public void MyMethod() 
    {
        ...
    }

}
```

