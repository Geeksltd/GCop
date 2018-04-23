# GCop 176

> *"This anonymous method should not contain complex code, Instead call other focused methods to perform the complex logic"*

## Rule description

Lambda expressions (aka anonymous methods) are a great convinience in C#, allowing you to create quick function expressions without the full ceremony of creating a method.

However, a very long lambda expression will make the code look complex and cluttered. Therefore you should only use lambda expressions for simply functions, and revert back to a full standard method if the code is long or complex.


## Example

```csharp
var customers = cacheManager.GetOrAdd(cacheKey, () =>
{
    // ... Complex implementation
    return result;
});
```

*should be* 🡻

```csharp
var customers = cacheManager.GetOrAdd(cacheKey, () => LoadCustomers());
...

IEnumerable<Customer> LoadCustomers()
{
   // ... Complex implementation
   return result;
}

```
