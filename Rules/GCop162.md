# GCop162

> *"Replace with **.IsAnyOf()**"*


## Rule description
The IsAnyOf extension method is available on some types such as String and IEntity, as an alternative to a number of OR (||) checks.
It improves readability and writeability. Also it can potentially provide a better performance.

## Example 1
```csharp
if (result == "-1" || result == "-2")
 return false;
```
*should be* 🡻

```csharp
if (result.IsAnyOf("-1","-2"))
    return false;
```
 
## Example 2
```csharp
if (order.GetCurrentStatus() == OrdeStatus.Pending || order.GetCurrentStatus() == OrdeStatus.Processing || order.GetCurrentStatus() == OrdeStatus.Ready))
 return false;
```
*should be* 🡻

```csharp
if (order.GetCurrentStatus().IsAnyOf(OrdeStatus.Pending, OrdeStatus.Processing, OrdeStatus.Ready))
    return false;
```

