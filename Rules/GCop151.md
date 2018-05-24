# GCop 151

> *"Equals or == should be used"*

## Rule description

Since `string` is also an `IEnumerable<System.Char>` all the usual collection Linq methods show up on string objects in intellisense, and that can be confusing. The `IsEquivalentTo()` method is not a `string` operation, but rather an `IEnumerable<T>()` functionality which compares equivalence between two collections by comparing all their items one  by one. 

When you want to compare strings, you should use the `Equals()` method or `==` operator. Most importantly for `Database` operations, the latter allows the framework to run the criteria at the database engine level for a much better performance.

## Example

```csharp
var result = Database.GetList<Customer>(s => s.IP.IsEquivalentTo("someValue"));
```

*should be* 🡻

```csharp
var result = Database.GetList<Customer>(s => s.IP == "someValue");
```
