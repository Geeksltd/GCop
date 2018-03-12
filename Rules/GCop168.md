# GCop168

> *"Don't instantiate a variable with the new keyword if you are going to assign it to a different object immediately."*


## Rule description
An object is created (and hence referenced) when an object variable is declared using the keyword new or when an object variable is assigned an existing object. So one of these assigns is not usable and should be ommited. 

## Example 1
```csharp
public static List<ShopProduct> FetchProducts()
{
    var result = new List<ShopProduct>();
    
    result = db.ShopProducts.Where(p => p.IsActive).ToList();
    //...
    return result;
}
```
*should be* 🡻

```csharp
public static List<ShopProduct> FetchProducts()
{    
    var result = db.ShopProducts.Where(p => p.IsActive).ToList();
    //...
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
    var myVar = new ShopProduct();
    myVar.Price = 100;
    myVar = result.FirstOrDefault();
}
```
