# GCop126

> *"To handle both null and empty string scenarios, use **IsEmpty / HasValue** instead"*


## Rule description
You can avoid unnecessary clutter in your code by using **IsEmpty** or **HasValue** instead of string.IsNullOrEmpty() or string.IsNullOrWhiteSpace().

## Example 1
```csharp
public static void MyMethod(string myParam)
{
  if (string.IsNullOrEmpty(myParam) || string.IsNullOrWhiteSpace(myParam))
    {
        //Several lines of code       
    }
}
```
*should be* 🡻

```csharp
public static void MyMethod(string myParam)
{
   if (myParam.TriOrEmpty().IsEmpty())
   {
       //Several lines of code             
   }
}
```
 ## Example 2
If your logic applies to null, but not empty string, then change the condition to ReferenceEquals.
```csharp
if (token != null)
{
    //several lines of code
}
```
*should be* 🡻

```csharp
if (! (token is null))
{
    //several lines of code
}
```

