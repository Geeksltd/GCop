# GCop 320

> *"While \{IEnumerable\<T>} is being enumerated at run-time, saving of each of its items can potentially affect the source expression from which this IEnumerable object is define upon. To avoid unintended side effects, evaluate myobjects into an Array or List before passing it to Database.XXX."*

## Rule description

M# runs bulk saves in “Transaction”. Meaning, failure to save one records results rolling back all the saved instances of the current collection. Whenever an entity instance is saved or updated, it goes through a series of events, which are used to implement core business logic e.g. validation. Saving of each of its items can potentially affect the source expression from which this `IEnumerable` object is define upon. To avoid unintended side effects, evaluate myobjects into an Array or List before passing it to `Database.Save()` or `Database.Update()`.

## Example1

```csharp
var products = AllProducts.Where(u => u.IsMostExpensiveInCategory());
Database.Update(products, s => s.ReducePriceBy(0.2));

// Imagine that the first two products in the list belong to the same category.
// The first one was £100 and the second one was £90. We intended to apply the 
// discount only to the most expensive item per category, which is the first product.
// But with this implementation, we first apply the discount to the first product, 
// making it £80. But then the IsMostExpensiveInCategory() method is evaluated on the 
// second product, which will now return true, meaning it also gets a discount applied to it.

// What we intended was to first identify the products that are the most expensive in their
// category before the change operation is applied, which is why we should have evaluated 
// the WHERE criteria before we start to apply the database change.

```

*should be* 🡻

```csharp
var products = AllProducts.Where(u => u.IsMostExpensiveInCategory()).ToArray();
Database.Update(products, s => s.ReducePriceBy(0.2));
```
