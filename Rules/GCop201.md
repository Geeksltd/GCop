# GCop201

> *"Use camelCasing when declaring local variables / a parameter."*


## Rule description

To capitalize some kind of identifiers in C#, we should use the CamelCase convention.
in CamelCase capitalization, the first letter of an identifier is lowercase and the first letter of each subsequent concatenated word is capitalized.

We should use CamelCasing for these identifires :

* instance fields
* Parameters
* Local variables

## Example 1

```csharp
public class CalendarEntry
{
    private DateTime Date;
}
```
*should be* 🡻

```csharp
public class CalendarEntry
{
    private DateTime date;
}
```

## Example 2

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

## Example 3

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

