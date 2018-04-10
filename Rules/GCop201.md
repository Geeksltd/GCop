# GCop 201

> *"Use camelCasing when declaring local variables / a parameter."*

## Rule description

Camel Case convention is used to capitalize some kind of identifiers in C#. In Camel Case capitalization, the first letter of an identifier is lowercase and the first letter of each subsequent concatenated word is capitalized.

Camel Casing is used for these identifiers :

* Parameters
* Local variables
* Class fields (only where a property or method already exists with the Pascal Cased version of that name)

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

## Example 3

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