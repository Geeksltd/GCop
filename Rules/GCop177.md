# GCop 177

> *"Variable declaration is unnecessary due to it being used only for return statement"*

## Rule description

It is a bad practice to declare a variable only to immediately `return` or `throw`. This variable is an internal implementation detail that is not exposed to the callers of the method so it does not improve code readability.

## Example

```csharp
public int Foo(int bar)
{
    var result = bar * 200;
    return result;  
}
```

*should be* 🡻

```csharp
public int Foo(int bar)
{
    return bar * 200; 
}
```
*which, in this case, allows it to get even shorter* 🡻
```csharp
public int Foo(int bar)=> bar * 200; 
```
