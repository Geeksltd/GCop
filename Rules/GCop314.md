# GCop 314

> *"You don't need the `Where` clause. Replace with `foo.FirstOrDefault(...)`"*

## Rule description

Using `Where` with `FirstOrDefault` clause or using `FistOrDefault` alone will make the same SQL statement. While the second approach is shorter and more meaningful.

## Example

```csharp
var myObj = foo.Where(a => a.Id == id).FirstOrDefault();
```

*should be* 🡻

```csharp
var myObj = foo.FirstOrDefault(a => a.Id == id);
```
