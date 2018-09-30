# GCop 127

> *"Use \{CollectionName}.IDs() instead of \{ColectionName.select}"*

## Rule description

The `IDs()` returns all entity Guid IDs for specified collection. it is more readable and much more comprehensible than using `Select()`.

## Example

```csharp
var myResult = myCollection.Select(s => s.ID);
```

*should be* 🡻

```csharp
var myResult = myCollection.IDs();
```