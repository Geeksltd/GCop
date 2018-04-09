# GCop 635

> *"The condition of the where clause is very long and should be turned into a method."*

## Rule description

The long where clause conditions will reduce code readability and makes refactoring harder. The long expression perhaps represents an abstraction which is missing. By refactoring that into a method, you will be documenting that abstraction. In addition, you might find that new method reusable.

## Example

```csharp
return shops.Where(s => s.AccountId == currentAccount.ID &&
                        s.Status == (int)ShopStatus.Approved && 
                        s.Title == "myTitle");
```

*should be* 🡻

```csharp
return shops.Where(s => IsSomething(s));

bool IsSomething(Shop shop)
{
     return shop.AccountId == CurrentAccount.ID && 
            shop.Status == (int)ShopStatus.Approved && 
            shop.Title == "myTitle");
}
```