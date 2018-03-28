# GCop635

> *"The condition of the where clause is very long and should be turned into a method."*


## Rule description
The long where clause conditions will reduce code readability and makes refactoring harder. The long expression perhaps represents an abstraction which is missing. By refactoring that into a method, you will be documenting that abstraction.

## Example 1
```csharp
return shops.Where(s => s.AccountId == currentAccount.ID && s.Status == (int)ShopStatus.Approved && s.Title == "myTitle");
```
*should be* 🡻

```csharp
return shops.Where(s => IsSomething(s));

static bool IsSomething(Shop shop)
{
     return shop.AccountId == currentAccount.ID && shop.Status == (int)ShopStatus.Approved && shop.Title == "myTitle");
}

// Or ideally move IsSomething() to the Shop class so it's not static

```
