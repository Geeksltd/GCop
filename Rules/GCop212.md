# GCop212

> *"Do not use addr as abbreviation for method parameter"*
> 
> *"Do not use res as abbreviation for method parameter"*


## Rule description
There is a rule about abbreviations that emphsized on not using abbreviations in parameter names becuase it reduces the readability of the code.

## Example 1
```csharp
var myObj = myList.Where(res => res.IsFree == true);
```
*should be* 🡻

```csharp
var myObj = myList.Where(list => list.IsFree == true);
```

 ## Example 2
```csharp
public void SetAddress(string addr)
{
    //do something
}
```
*should be* 🡻

```csharp
public void SetAddress(string address)
{
    //do something
}```
