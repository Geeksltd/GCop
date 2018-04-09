# GCop 161

> *"Since the {left / right} side of condition is just Bool, then \" == true\" is unnecessary"*

## Rule description
To have a short and readable code it is better not to write "== true" for *Boolean* objects.There is an exception in this rule for *NullableBoolean* objects.

## Example 1

```csharp
if (notNullableBoolVar == true)
{
    ...
}
```

*should be* 🡻

```csharp
if (notNullableBoolVar)
{
    ...
}
```

## Example 2

```csharp
var result = myIEnumarable.Where(mi => mi.IsActive == true).ToList(); 
```

*should be* 🡻

```csharp
var result = myIEnumarable.Where(mi => mi.IsActive).ToList();
```