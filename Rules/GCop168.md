# GCop 168

> *"Don't instantiate a variable with the **new** keyword if you are going to assign it to a different object immediately."*

## Rule description

Variable definition is separate from object creation. When using the `new` keyword, a new object is actually created in the memory which wastes memory and garbage collection's time. When a variable is immediately assigned to another object, there is no point on instantiating it first. 

## Example 1

```csharp
public static List<ShopProduct> FetchProducts()
{
    var result = new List<ShopProduct>();
    
    result = db.ShopProducts.Where(p => p.IsActive).ToList();
    ...
    return result;
}
```

*should be* 🡻

```csharp
public static List<ShopProduct> FetchProducts()
{    
    var result = db.ShopProducts.Where(p => p.IsActive).ToList();
    ...
    return result;
}
```

## Example 2

```csharp
public void MyMethod()
{
    var result = db.ShopProducts.Where(p => p.IsActive).ToList();
    var myVar = new ShopProduct();
    myVar = result.FirstOrDefault();
}
```

*should be* 🡻

```csharp
public static List<ShopProduct> FetchProducts()
{   
    var result = db.ShopProducts.Where(p => p.IsActive).ToList(); 
    var myVar = result.FirstOrDefault();
    ...
}
```