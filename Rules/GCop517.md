# GCop 517

> *"{'MethodName()'} returns a value but doesn't change the object. It's meaningless to call it without using the returned result."*

## Rule description

When `void` is used as a return type for a method, it  specifies that the method doesn't return a value and doesn’t change the object. So it is meaningless to use `void` while the method changes the object.

## Example

```csharp
public void AddHours(double hours)
{
    CurrentDateTime.AddHours(hours);
}
```

*should be* 🡻

```csharp
public datetime AddHours(double hours)
{
    return CurrentDateTime.AddHours(hours);
}
```
