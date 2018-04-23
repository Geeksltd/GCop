# GCop 605

> *"Use \{parameterName}.To<\{toType}>() instead."*

## Rule description

...

## Example

```csharp
public static object StringToType(string value, Type propertyType)
{
    return Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture);
}
```

*should be* 🡻

```csharp
public static object StringToType(string value, Type propertyType)
{
    return value.To(propertyType);
}
```
