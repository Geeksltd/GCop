# GCop634

> *"Instead of private property, use a class field."*


## Rule description
Fields should (almost always) be kept private to a class and accessed via get and set properties. 

Properties are public and provide access to private fields. So have a private property is meaningless.
## Example 1
```csharp
public class MyClass
{
    private string MyProperty{ get; set; }
}
```
*should be* 🡻
```csharp
public class MyClass
{
    // this is a field.  It is private to your class and stores the actual data.
    private string _myField;

    public string MyProperty
    {
        get
        {
            return _myField;
        }
        set
        {
            _myField = value;
        }
    }
}
```
