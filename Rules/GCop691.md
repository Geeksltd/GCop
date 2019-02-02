# GCop 691

> *"Non-public methods should not need basic argument validation."*

## Rule description

The main purpose of validating arguments in methods is to inform the callers that they are using the method incorrectly. Therefore, this should be done only when the calling code is not in your control.
Callers of non-public methods are always in your control. If the method is called with invalid arguments, it is a logical error in the calling code, which is within your control. Thus, that should be fixed at the source of the problem.

## Example

```csharp
private void Bar(FooBar foo)
{
    if (foo == null)
        throw new ArgumentNullException(nameOf(foo));
    
    // use the parameter...
}
```

*should be* 🡻

```csharp
private void Bar(FooBar foo)
{        
    // use the parameter...
}
```
