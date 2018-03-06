# GCop201

> *"Use camelCasing when declaring local variables / a parameter."*


## Rule description

CamelCase convention is used to capitalize some kind of identifiers in C#. In CamelCase capitalization, the first letter of an identifier is lowercase and the first letter of each subsequent concatenated word is capitalized.

CamelCasing is used for these identifires :

* Parameters
* Local variables


## Example 1

```csharp
private void myMethod(int MyParameter)
{  
    ...
}
```
*should be* 🡻

```csharp
private void myMethod(int myParameter)
{  
    ...
}
```

## Example 2

```csharp
private void myMethod()
{
    int MyLocalVar = 3;
    ...
}
```
*should be* 🡻

```csharp
private void myMethod()
{
    int myLocalVar = 3;
    ...
}
```

