# GCop 526

> *"Use the Generic version instead. Ensure you have `using System.Reflection;`"*

## Rule description

The `.GetCustomAttribute<T>()` method belongs to the `CustomAttributeExtensions` extension class which contains very thin wrappers to `Attribute.GetCustomAttribute()` and `Attribute.GetCustomAttributes()`. These wrappers cast the returned `Attribute` to the expected attribute type. 

## Example

```csharp
var res = someType.GetCustomAttributes(typeof(CLSCompliantAttribute), true)
            .SingleOrDefault() as CLSCompliantAttribute;
```

*should be* 🡻

```csharp
var res = someType.GetCustomAttribute<CLSCompliantAttribute>();
```
