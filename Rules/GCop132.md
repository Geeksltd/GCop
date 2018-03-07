# GCop132

> *"Since the type is inferred, use 'var' instead"*


## Rule description
The potential benefit of using var instead of type is in readability . when the type of the variable is clear on the RHS of the assignment (e.g. via a cast or a constructor call), there is no benefit of also having it on the LHS.

There are some exception in this rule:

 * If the left hand side type is any of (Boolean, Decimal, Int32, String, Int64, Char)
 * If the left hand side type is any of (Action, Func)
 * If the left or right hand side type is null.

## Example 1
```csharp
public void MyMethod()
{
    object myObject = new object();
    //some other codes
}
```
*should be* 🡻

```csharp
public void MyMethod()
{
    var myObject = new object();
    //some other codes
}
```
 

## Example 2
```csharp
public void MyMethod()
{
    Dictionary<int, MyLongNamedObject> dictionary = new Dictionary<int, MyLongNamedObject>();
}
```
*should be* 🡻

```csharp
public void MyMethod()
{
    var dictionary = new Dictionary<int, MyLongNamedObject>();
}
```
 