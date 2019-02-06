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
void Foo(int Bar)
{  
    ...
}
```

*should be* 🡻

```csharp
void Foo(int bar)
{  
    ...
}
```

## Example 2

```csharp
void Foo()
{
    int LocalBar = 3;
    ...
}
```

*should be* 🡻

```csharp
void Foo()
{
    int localBar = 3;
    ...
}
```

## Example 3

```csharp
int _Foo;
public int Foo
{ 
    get => _Foo;
    set => _Foo = value;
}
```

*should be* 🡻

```csharp
int foo;
public int Foo
{ 
    get => foo;
    set => foo = value;
}
```