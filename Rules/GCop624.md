# GCop 624

> *"Write it as  \{CollectionName}.\{Contains}(VALUE)"*
> 
> *"Write it as  \{CollectionName}.\{Except}(VALUE).Any()"*

## Rule description

The `someCollection.Contains()` method takes an object while `Any(...)` takes a predicate. So if you want to check for existence of an element, use `Contains()` rather than comparing every item using `Any(...)`.

## Example

```csharp
if (myCollection.Any(c => c == categoryId))
{
    ...
}
```

*should be* 🡻

```csharp
if (myCollection.Contains(categoryId))
{
    ...
}
```

> TODO: Break into another rule: https://github.com/Geeksltd/GCop/issues/108

## Example2

```csharp
if (myCollection.Any(c => c != categoryId))
{
    ...
}
```

*should be* 🡻

```csharp
if (myCollection.Except(categoryId).Any())
{
    ...
}
```
