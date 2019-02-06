# GCop 136

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

## Example

```csharp
public class Bar
{
    public void Foo() 
    {
        ...
    }
    private const string fooBar = "someText";

}
```

*should be* 🡻

```csharp
public class Bar
{
    private const string fooBar = "someText";
    
    public void Foo() 
    {
        ...
    }

}
```