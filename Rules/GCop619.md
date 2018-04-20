# GCop 619

> *"Should be written "{'conditionText'}"."*

## Rule description

To have an invert assignment of `boolean` objects it’s better not to use `if` conditions.

## Example

```csharp
if (someValue == 0)
    return false;
else
    return true;
```

*should be* 🡻

```csharp
return !(someValue == 0);
```

*OR* 🡻

```csharp
return someValue != 0;
```

