# GCop201

> *"Use camelCasing when declaring local variables / a parameter."*


## Rule description

CamelCase convention is used to capitalize some kind of identifiers in C#. In CamelCase capitalization, the first letter of an identifier is lowercase and the first letter of each subsequent concatenated word is capitalized.

CamelCasing is used for these identifires :

* Parameters
* Local variables
* Class fields (only where a property or method already exists with the PascalCased version of that name)

## Example 1

```csharp
void MyMethod(int MyParameter)
{  
    ...
}
```
*should be* 🡻

```csharp
void MyMethod(int myParameter)
{  
    ...
}
```

## Example 2

```csharp
void myMethod()
{
    int MyLocalVar = 3;
    ...
}
```
*should be* 🡻

```csharp
void myMethod()
{
    int myLocalVar = 3;
    ...
}
```

## Example 4

```csharp
int _SomeName;
public int SomeName
{ 
    get => _SomeName;
    set => _SomeName = value;
}
```
*should be* 🡻

```csharp
int someName;
public int SomeName
{ 
    get => someName;
    set => someName = value;
}
```
