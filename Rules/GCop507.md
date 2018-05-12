# GCop 507

> *"Remember to implement this method."*

## Rule description

The `NotImplementedException` is not a debugging construct, but it should not be thrown in completed programs. When adding a method, you may want the method to exist but you may not be able to implement it yet.

GCop will show a central list of those to help you remember and address them.

## Example

```csharp
public static string MyMethod()
{
    throw new NotImplementedException();
}
```