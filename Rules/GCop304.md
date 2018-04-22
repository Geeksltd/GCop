# GCop 304

> *"Property named \{ParameterName}Id should be used instead"*

## Rule description

...

## Example

```csharp
 var addresses = myCollection.Where(mc => mc.Country == null).ToList();
```

*should be* 🡻

```csharp
 var addresses = myCollection.Where(mc => mc.CountryId == null).ToList();
```