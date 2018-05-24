# GCop 321

> *"The variable \{VariableName} is enumerated multiple times. To improve performance, call `.ToList()` where it's defined."*
> 
> *"\{VariableName} is enumerated multiple times. To improve performance, define a variable before the loop and set to `{VariableName}.ToList()` and use that variable in the loop."*
## Rule description

An object of type IEnumerable can be resource consuming every time that it's iterated in foreach loops or Linq methods that evaluate the result. When you use that inside a loop or another Linq query, it's better to call .ToArray() or .Tolist() on it once to prepare the final result, and then use that directly.

## Example

```csharp
var theResource = someSource.Where(s => s.SomethingThatTakesTime());

foreach(var item in someList)
{
    // The WHERE criteria declared above is executed multiple times, once for each item in someList.    
    doSomethignWith(theResource);    
}
```

*should be* 🡻

```csharp
var theResource = someSource.Where(s => s.SomethingThatTakesTime()).ToArray();

foreach(var item in someList)
{
    // The WHERE criteria declared above is only executed once. 
    doSomethignWith(theResource);    
}
```
