# GCop 217

> *"Rename the method to Is...Valid"*

## Rule description

A validation method named as "ValidateSomething()" is expected to be void, and throw an exception if necessary.

A validation method named as "IsSomethingValid()" is expected to not throw an exception, and instead return true/false.

## Example

```csharp
public bool ValidateOrder()
{
    ...
}
```

*should be* 🡻

```csharp
public bool IsOrderValid()
{
    ...
}
```