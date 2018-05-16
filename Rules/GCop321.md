# GCop 321

> *"The variable \{VariableName} is enumerated multiple times. To improve performance, call `.ToList()` where it's defined."*
> 
> *"\{VariableName} is enumerated multiple times. To improve performance, define a variable before the loop and set to `{VariableName}.ToList()` and use that variable in the loop."*
## Rule description

An object of type IEnumerable can be resource consuming every time that it's iterated in foreach loops or Linq methods that evaluate the result such as ToList(), ToArray(), Count(), Any(), ...

If you need the result of an IEnumerable object more than once, it's better to call .ToArray() or .Tolist() on it once to prepare the final result, and then use that directly.

## Example

```csharp
foreach(customer item in customerCollection)
{
    var res = customerCollection.Where(s => s.Name == "").ToList();
    ...
}
```

*should be* 🡻

```csharp
var customerList = customerCollection.ToList();
foreach(var item in customerList)
{
    var res = customerList.Where(s => s.Name == "");
    ...
}
```