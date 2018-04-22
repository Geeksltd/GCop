# GCop 304

> *"Property named \{ParameterName}Id should be used instead"*

## Rule description

...

## Example

```csharp
 var addresses = myCollection.Where(a => a.Country == null).ToList();
```

*should be* 🡻

```csharp
(...corrected version)
```