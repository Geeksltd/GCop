# GCop 615

> *"Negative logical comparisons are taxing on the brain. Instead of "!object.Any()" use "object.None()"."*

## Rule description

Comparisons are more clear and meaningful while we dont use negative syntax.

## Example 1

```csharp
if(!myObj.HasValue)
{
    ...
}
```

*should be* 🡻

```csharp
if(myObj == null)
{
    ...
}
```
## Example 2

```csharp
if(!data.Any(d => d.ProductCode == 120))
{
    ...
}                    
```

*should be* 🡻

```csharp
if(data.None(d => d.ProductCode == 120))
{
    ...
} 
```