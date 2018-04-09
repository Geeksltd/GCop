# GCop 162

> *"Replace with **.IsAnyOf()**"*

## Rule description

The `IsAnyOf` extension method is available on some types such as `String` and `IEntity`, as an alternative to a number of `OR (||)` checks.
It improves readability and writ ability. Also it can potentially provide a better performance.

## Example 1

```csharp
if (result == "something" || result == "another-thing")
   return false;
```

*should be* 🡻

```csharp
if (result.IsAnyOf("something","another-thing"))
   return false;
```
 
## Example 2

```csharp
if (order.GetCurrentStatus() == OrdeStatus.Pending || order.GetCurrentStatus() == OrdeStatus.Processing ||
    order.GetCurrentStatus() == OrdeStatus.Ready))
   return false;
```

*should be* 🡻

```csharp
if (order.GetCurrentStatus().IsAnyOf(OrdeStatus.Pending, OrdeStatus.Processing, OrdeStatus.Ready))
   return false;
```

