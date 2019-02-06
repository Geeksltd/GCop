# GCop 204

> *"Rename the variable `VariableName` to something clear and meaningful."*

## Rule description

This error will be shown when your variable or parameter name is a single character. Variable and method parameter names should be descriptive enough to reveal their meaning, purpose and role in that context.

There are some exception in this rule:
 
  * When the parameter is of type `EventArgs` it can be a single character (usually 'e') for historic reasons.
  * Lambda expression variables
  * Iterator variable in small for loops
  
## Example 1

```csharp
void Foo(DateTime d)
{
    ...
}
```

*should be* 🡻

```csharp
void Foo(DateTime date)
{
    ...
}
```

## Example 2

```csharp
void Foo()
{
    var d = ...;
}
```

*should be* 🡻

```csharp
void Foo()
{
    var date = ...;
}
``` 

## Example 3

```csharp
public class Foo
{
    public string F;    
}
```

*should be* 🡻

```csharp
public class Foo
{
    public string Family;   
}
```
