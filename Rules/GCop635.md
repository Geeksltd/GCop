# GCop635

> *"The condition of the where clause is very long and should be turned into a method."*


## Rule description
The long where clause conditions will reduce code readability and makes refactoring harder.

## Example 1
```csharp
var myObj = Db.EntityName.Where(s => s.AccountID == currentAccount.ID && s.Status == (int)Engine.Data.Shop.ShopStatus.Approved && s.Title == "myTitle");
```
*should be* 🡻

```csharp
System.Linq.Expressions.Expression<Func<Engine.Data.EntityName, bool>> predicate = s =>
                    s.AccountID == (CurrentAccount.ID) &&
                    s.Status == ((int)Engine.Data.Shop.ShopStatus.Approved) &&
                    s.Title == "myTitle" ;

var myObj = Db.EntityName.Where(predicate);
```
