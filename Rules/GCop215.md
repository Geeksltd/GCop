# GCop215

> *"Rename the method to 'Count...' as it's shorter and more readable"*


## Rule description
It is more meaningful to write *CountSomething* as a method name than writing *GetSomethingCount* when the method is realy counting something. 

## Example 1
```csharp
public int GetCustomersCount()
{
    ...
}
```
*should be* 🡻

```csharp
public int CountCustomers()
{
    ...
}
```