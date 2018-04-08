# GCop404

> *"Multiple 'if' and 'else if' on the same variable can be replaced with a 'switch'"*


## Rule description
Where multiple subsequent `if (...)` conditionss are checking the same variable against different values, prefer `switch (...)` over `if (...)`, because it is more readable and faster to run.

A `switch` is really saying *"pick one of these based on this variables value"* (using a lookup table or a hash list for more than five items) but an `if` statement is just a series of boolean checks.

## Example 1
```csharp
if(role == 0)
{
    ...
}
else if(role == 1)
{
    ...
}
else if(role == 2)
{
    ...
}
```
*should be* 🡻

```csharp
switch(role)
{
    case 0:
    ...
    break;
    case 1:
    ...
    break;
    case 2:
    ...
    break;
    default:
    throw ArgumentException;
}
```
