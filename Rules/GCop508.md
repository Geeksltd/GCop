# GCop508

> *"Since the condition is already a boolean it can be returned directly."*


## Rule description
There's no need to explicitly return true or false. It is most readable to return boolean directly.

## Example 1
```csharp
var isFree = cost == 0 ? false : true;
```
*should be* 🡻

```csharp
var isFree = cost == 0 ;
```
