# GCop206

> *"Avoid using underscores in the class name"*

> *"Avoid using underscores in a local method variable declaration"*


## Rule description
Underscore should be avoided in most cases. The only exception where it's allowed (but not recommended) is for naming private class fields that back a property. 

## Example 1
```csharp
void MyMethod()
{
    var _myVariable = "someText";
}
```
*should be* 🡻

```csharp
void MyMethod()
{
    var myVariable = "someText";
}
```
 

## Example 2
```csharp
public class Some_Class
{
    ...
}
```
*should be* 🡻

```csharp
public class SomeClass
{
    ...
}
```
 
 

