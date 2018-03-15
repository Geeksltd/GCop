# GCop157

> *"Use == instead"*


## Rule description
The “==” works with nulls but “Equals” crashes when you compare NULL values.

The “==” does type checking during compile time while “Equals” is more during runtime.


## Example 1
```csharp
var result = myList.SingleOrDefault(s => s.Name.Equals("onlinepayment"));
```
*should be* 🡻

```csharp
var result = myList.SingleOrDefault(s => s.Name == "onlinepayment");
```

## Example 2
```csharp
var city = "london";
if (city.Equals("tehran"))
{
    //...
}
```
*should be* 🡻

```csharp
var city = "london";
if (city == "tehran")
{
    //...
}
```