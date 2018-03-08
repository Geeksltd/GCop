# GCop148

> *"All constructors should be before all methods in a class."*


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
public partial class Banner
{
    public void MyMethod() 
    {
        //some codes
    }
    public Banner()
    {
        //some codes
    }       
}
```
*should be* 🡻

```csharp
public partial class Banner
{
    public Banner()
    {
        //some codes
    } 
    public void MyMethod() 
    {
        //some codes
    }
}
```

