# GCop 528

> *"Use IsA() or IsA\<T>() instead."*

## Rule description

The `Type.IsAssignableFrom(Type)` method, determines whether an instance of a specified type can be assigned to an instance of the current type. It is more readable to use `IsA()` or `IsA<T>` instead.

## Example

```csharp
var result = someType.IsAssignableFrom(anotherType);
```

*should be* 🡻

```csharp
var result = someType.IsA(anotherType);
```