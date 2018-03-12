# GCop318

> *"This will cause the query to be computed multiple times. Instead call .ToList() on the variable declaration line to avoid unwanted extra processing."*


## Rule description
If foreach loop is used on IEnumerable  object, there would be a cast in each loop that is time consuming.So its better to call .Tolist() on the object so there wouldnt be any need to cast .

## Example 1
```csharp
private void MyMethod()
{
    var children = allcategories.Where(lai => lai.ParentID == Parent.CategoryID);
    foreach (var Child in children)
    {
        //do somethings
    }
}
```
*should be* 🡻

```csharp
private void MyMethod()
{
    var children = allcategories.Where(lai => lai.ParentID == Parent.CategoryID).ToList();
    foreach (var Child in children)
    {
        //do somethings
    }
}
```

