# GCop157

> *"Use == instead"*


## Rule description
There is no difference between equality comparison using “==” and “Equals()”, except when you are comparing **String** comparison.

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

## Example 3
```csharp
object str1 = new string(newchar[] { 't', 'e', 's', 't' });
object str2 = new string(newchar[] { 't', 'e', 's', 't' });
Console.WriteLine(str1==str2); // false
Console.WriteLine(str1.Equals(str2));  // true
```

## Example 4
```csharp
object str1 = "test";
object str2 = "test";
Console.WriteLine(str1==str2);//true
Console.WriteLine(str1.Equals(str2));//true

```