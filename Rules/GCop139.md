# GCop 139

> *"Use constant instead of field."*

## Rule description

A constant member is defined at compile time and cannot be changed at runtime. `const` and `static readonly` perform a similar function on data members. If you know the value before compile time, it is more common to use `constant`.

## Example

```csharp
private static readonly int someVariable = 0;
```

*should be* 🡻

```csharp
private const int someVariable = 0;
```
