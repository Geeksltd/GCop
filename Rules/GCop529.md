# GCop 529

> *"Use 'IEnumerable< MyClassObject >' instead."*

## Rule description

When your code only needs to read data out of the parameter use `IEnumerable` type rather than List. Using the `IEnumerable` type for an argument tells the caller that this object is used as read-only. On the other hand, taking a List parameter in, may confuse the caller to think that the method might change it (add or remove items).

## Example

```csharp
public MyMethod(List<Product> products)
{
    // ... code that only reads products
}
```

*should be* 🡻

```csharp
public MyMethod(IEnumerable<Product> products)
{
    // ... code that only reads products
}
```