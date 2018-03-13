# GCop401

> *"Instead of setting the properties in separate lines, use constructor initializers."*


## Rule description
Object initializers let you assign values to any accessible fields or properties of an object at creation time without having to invoke a constructor followed by lines of assignment statements.

## Example 1
```csharp
var myObj = new MyClassName();
myObj.Property1 = "somethings";
myObj.Property2 = 2;
myObj.Property3 = 1.2;
```
*should be* 🡻

```csharp
var myObj = new MyClassName
{
    Property1 = "somethings";
    Property2 = 2;
    Property3 = 1.2;
};
```
 
