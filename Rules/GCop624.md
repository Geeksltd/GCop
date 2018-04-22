# GCop 624

> *"Write it as  \{CollectionName}.\{Contains}(VALUE)"*
> 
> *"Write it as  \{CollectionName}.\{Except}(VALUE).Any()"*

## Rule description

`Contains` takes an object while `Any` takes a predicate. So if you want to check for a specific condition, use `Any`. If you want to check for the existence of an element, use `Contains`.

## Example1

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