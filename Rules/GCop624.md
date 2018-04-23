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

> TODO: Move to another rule

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
