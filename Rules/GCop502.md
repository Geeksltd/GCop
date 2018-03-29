# GCop502

> *"The parameter 'ParameterName' doesn't seem to be used in this method. Consider removing it. If the argument must be declared for compiling reasons, rename it to contain only underscore character."*


## Rule description
Unused parameters just increase code length and reduce readability of code, so it is better to avoid using theme.

## Example 1
```csharp
public void MyMethod(MyClassObject myProperty)
{
    ...
    //myProperty is Not used in the method body
}
```
*should be* 🡻

```csharp
public void MyMethod()
{
    ...
}
```
