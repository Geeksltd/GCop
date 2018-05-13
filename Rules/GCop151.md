# GCop 151

> *"Equals or == should be used"*

## Rule description

The `IsEquivalentTo` method determines whether this list is equivalent to another specified list. While `Equals` or `==` determine whether this instance and another specified `String` object have the same value.

## Example

```csharp
var result = Database.GetList<Customer>(s => s.IP.IsEquivalentTo("someValue"));
```

*should be* 🡻

```csharp
var result = Database.GetList<Customer>(s => s.IP == "someValue");
```
