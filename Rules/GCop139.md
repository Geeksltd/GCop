# GCop 139

> *"Use constant instead of field."*

## Rule description

A constant member is defined at compile time and cannot be changed at run-time. `const` and `static readonly` perform a similar function on data members. If you know the value before compile time, it is more common to use constant.

## Example

```csharp
private static readonly int foo = 0;
```

*should be* 🡻

```csharp
private const int foo = 0;
```