# GCop120

> *"Use HoursCount.Hours() instead of TimeSpan(HoursCount)"*


## Rule description
...

## Example 1
```csharp
Thread.Sleep(TimeSpan.FromHours(2));
```
*should be* 🡻

```csharp
Thread.Sleep(2.Hours());
```