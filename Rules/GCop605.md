# GCop 605

> *"Use \{parameterName}.To<\{toType}>() instead."*

## Rule description

The `string.To(...`) extension method is faster and smarter than than the standard `Convert.ChangeType`, and it's more readable.

## Example

```csharp
return Convert.ChangeType(value, propertyType);
```

*should be* 🡻

```csharp
return value.To(propertyType);
```
