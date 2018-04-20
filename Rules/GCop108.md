# GCop 108

> *"Instead of GetValueOrDefault(defaultValue) method use \" ?? defaultValue\"."*

## Rule description

the `GetValueOrDefault()` retrieves the value of the current `Nullable<T>` object, or the object's default value.
it is the same as when you call Null Coalescing Operator, `Nullable<T> ?? defaultValue`, while the first one is unclear when the condition is true(one must know the default value of `T`).

## Example1

```csharp
var result = intNullableObj.GetValueOrDefault();
```

*should be* 🡻

```csharp
var result = intNullableObj ?? 0;
```

## Example2

```csharp
var result = boolNullableObj.GetValueOrDefault();
```

*should be* 🡻

```csharp
var result = boolNullableObj ?? false;
```