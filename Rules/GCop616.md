# GCop616

> *"Reverse your IF criteria and use 'continue'. That will eliminate the need for a big IF block and make the code more readable."*


## Rule description
You can use the continue statement to avoid deeply nested conditional code, or to optimize a loop by eliminating frequently occurring cases that you would like to reject.

## Example 1
```csharp
foreach (var item in myList)
{
    if(item != null)
    {
        //write lots of codes
    }
}
```
*should be* 🡻

```csharp
foreach (var item in myList)
{
    if(item == null)
        continue;
    //write lots of codes
}
```

