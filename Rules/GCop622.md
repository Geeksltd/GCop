# GCop622

> *"Reverse your IF condition and return. Then move the nested statements to after the IF."*


## Rule description
It is better to get out of the method immediately if it makes the intent of the code more clear. In this way you will have a more meaningful code.

## Example 1
```csharp
if (some condition)
{
    Some code...            
}
```
*should be* 🡻

```csharp
if (!some condition) return;
   Some code...
```