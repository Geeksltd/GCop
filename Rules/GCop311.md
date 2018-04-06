# GCop311

> *"Throw exception without specifying the original exception. Remove 'exceptionIdentifier' from throw statement."*


## Rule description
*Throw* re-throws the exception that was caught, and preserves the stack trace. *throw ex* throws the same exception, but resets the stack trace to that method.

Unless you want to reset the stack trace (i.e. to shield public callers from the internal workings of your library), throw is generally the better choice, since you can see where the exception originated.

## Example 1
```csharp
try
{
    ...
}
catch (Exception ex)
{
    throw ex;
}
```
*should be* 🡻

```csharp
try
{
    ...
}
catch (Exception)
{
    throw;
}
```