# GCop 634

> *"Instead of private property, use a class field."*

## Rule description

A private auto-property has no advantage to a simple class field. But it has two small disadvantages:
- It's slightly slower to invoke compared to a plain field.
- It has the *{get; set;}* noise code.

## Example

```csharp
public class MyClass
{
    private string MyProperty{ get; set; }
}
```

*should be* 🡻
```csharp
public class MyClass
{
     private string MyProperty;
     
     // Or just:
     string MyProperty;
}
```