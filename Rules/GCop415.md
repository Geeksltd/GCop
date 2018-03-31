# GCop415

> *"The same code is repeated in multiple IF branches. Instead update the IF condition to cover both scenarios."*


## Rule description
Repeated Statements will reduce code readability. To have a more meaningful code it is better to refactor these IF conditions.

## Example 1
```csharp
switch(myValue)
{
    case -1: Quality = Quality.VeryLow; break;
    case 0: Quality = Quality.Medium; break;
    case 1: Quality = Quality.High; break;
    default: Quality = Quality.VeryLow; break;    
}
```
*should be* 🡻

```csharp
switch(myValue)
{
    case 0: Quality = Quality.Medium; break;
    case 1: Quality = Quality.High; break;
    default: Quality = Quality.VeryLow; break;    
}
```

