# GCop 108

> *"Instead of GetValueOrDefault(defaultValue) method use \" ?? defaultValue\"."*

## Rule description

The `GetValueOrDefault()` method retrieves the value of the current `Nullable<T>` object, or the object's default value. It is the same as when you call the *null coalescing* Operator such as `Nullable<T> ?? defaultValue`.

This is briefer, but more importantly it's explicit about what default value will be used in case the nullable object was indeed `null`.

## Example1

```csharp
var result = myNullableInteger.GetValueOrDefault();
```

*should be* 🡻

```csharp
var result = myNullableInteger ?? 0;
```

## Example2

```csharp
return myNullableBool.GetValueOrDefault();
```

*should be* 🡻

```csharp
return myNullableBool ?? false;
```
