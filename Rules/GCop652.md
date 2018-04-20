# GCop 652

> *"if (\{myVariable}.Any()) is unnecessary when using foreach."*

## Rule description

Unless you have some logic after the foreach statement that you want to avoid, that's unnecessary to use `Any()` as it will work the same.

When foreach iterates over your variable, detects that there are no items and will exit the loop.

## Example

```csharp
if(myIQueryable.Any())
{
    foreach(var item in myIQueryable)
    {
        ...
    }
}
```

*should be* 🡻

```csharp
foreach(var item in myIQueryable)
{
    ...
}
```