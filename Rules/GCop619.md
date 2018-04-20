# GCop 619

> *"Should be written "{'conditionText'}"."*

## Rule description

You should use the boolean logic for shorter and more readable code. You don't need to use an `if` condition in many cases. This rule will catch a common pattern.

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

