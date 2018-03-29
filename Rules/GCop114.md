# GCop114

> *"DateTime.Now is not TDD friendly. Use LocalTime.Now instead."*


## Rule description
...

## Example 1
```csharp
var endDate = DateTime.Today.AddDays(1);
```
*should be* 🡻

```csharp
var endDate = LocalTime.Now.AddDays(1);
```
