# GCop 215

> *"Rename the method to 'Count...' as it's shorter and more readable"*

## Rule description

It is shorter and more readable to write *CountSomething* as a method name than writing *GetSomethingCount*. 

## Example

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