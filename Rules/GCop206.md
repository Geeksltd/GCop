# GCop206

> *"Avoid using underscores in the class name"*

> *"Avoid using underscores in a local method variable declaration"*


## Rule description
Underscore used only when defining the underlying private member variable for a public property. Other variables or classes would not have an underscore.

MSDN Class Naming Guidlines page mentions not to use the underscore character (_) for classes.
 

## Example 1
```csharp
private void MyMethod()
{
    string _myVariable = "someText";
}
```
*should be* 🡻

```csharp
private void MyMethod()
{
    string myVariable = "someText";
}
```
 
 

## Example 1
```csharp
public class _MyClass
{
    ...
}
```
*should be* 🡻

```csharp
public class MyClass
{
    ...
}
```
 
 

