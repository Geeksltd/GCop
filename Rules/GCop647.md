# GCop647

> *"Shorten this property by defining it as expression-bodied."*


## Rule description
If you choose to implement a property get accessor yourself, you can use an expression body definition for single expressions that simply return the property value.

## Example 1
```csharp
public string Property
{
    get { return "something"; }
}
```
*should be* 🡻

```csharp
public string Property => "something";
```

