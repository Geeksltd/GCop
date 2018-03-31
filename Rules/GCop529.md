# GCop529

> *"Use 'IEnumerable< MyClassObject >' instead."*


## Rule description
One important difference between IEnumerable and List (besides one being an interface and the other being a concrete class) is that IEnumerable is read-only and List is not.

If you need the ability to make permanent changes of any kind to your collection (add & remove), you'll need List. If you just need to read, sort and/or filter your collection, IEnumerable is sufficient for that purpose.


## Example 1
```csharp
public MyMethod(List<Product> products)
{
    ...
}
```
*should be* 🡻

```csharp
public MyMethod(IEnumerable<Product> products)
{
    ...
}
```