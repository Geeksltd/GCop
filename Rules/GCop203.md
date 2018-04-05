# GCop203

> *"Use meaningful names instead of single character for foreach identifier"*


## Rule description
This error will be shown when your *foreach identifier* is a single character. It should be descriptive engough to reveal its meaning, purpose and role in that context.

## Example 1
```csharp
foreach (int e in myArray)
{
    ...
}
```
*should be* 🡻

```csharp
foreach (int element in myArray)
{
    ...
}
```