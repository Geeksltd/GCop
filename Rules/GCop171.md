# GCop 171

> *"There is no need for calling .Value. Replace with 'Paarmeter without .Value'"*

## Rule description

In some situations, there is no need to use .Value from nullable Value types. In those cases, for cleanness of the code, the code should be simplified.

## Example

```csharp
public void MyMethod(DateTime? fromDate = null, DateTime? toDate = null)
{
    if(fromDate.Value > toDate.Value)
    {
        ...
    }
}
```

*should be* 🡻

```csharp
public void MyMethod(DateTime? fromDate = null, DateTime? toDate = null)
{
    if(fromDate > toDate)
    {
        ...
    }
}
```