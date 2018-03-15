# GCop162

> *"Replace with **.IsAnyOf()**"*


## Rule description
Use IsAnyOf() instead of long or comparisons improve code readabilty.

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
 

