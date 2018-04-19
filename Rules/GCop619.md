# GCop 619

> *"Should be written "{'conditionText'}"."*

## Rule description

To have an invert assignment of `boolean` objects it’s better not to use `if` conditions.
## Example1

```csharp
if (boolObject.IsActive) boolObject.IsActive = false;
```

*should be* 🡻

```csharp
boolObject.IsActive = !(boolObject.IsActive);
```

## Example2

```csharp
if (someValue == 0)
    return false;
else
    return true;
```

*should be* 🡻

```csharp
return !(someValue == 0)
```