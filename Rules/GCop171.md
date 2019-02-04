# GCop 171

> *"There is no need for calling `.Value`. Replace with `fromDate > toDate`"*

## Rule description

In some situations, there is no need to use `.Value` for nullable value types. In those cases, for cleanness of the code, the code should be simplified.

## Example

```csharp
public void Foo(DateTime? fromDate = null, DateTime? toDate = null)
{
    if(fromDate.Value > toDate.Value)
    {
        ...
    }
}
```

*should be* 🡻

```csharp
public void Foo(DateTime? fromDate = null, DateTime? toDate = null)
{
    if(fromDate > toDate)
    {
        ...
    }
}
```