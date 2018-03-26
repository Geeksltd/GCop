# GCop160

> *"This is not readable. Either refactor into a method, or use If / else statement."*


## Rule description
Writting long conditional expression will reduce code readability and increase misundrestanding.

If the length of conditional expression is more than 110 characters, this error will be hint.

## Example 1
```csharp
var code = currentAccount.ID + (currentProduct.ProductId.ToString().Length > 3 ? currentProduct.ProductId.ToString() : (currentProduct.ProductId.ToString().Length == 2 ? "00" + currentProduct.ProductId : "0" + currentProduct.ProductId));
```
*should be either* 🡻

```csharp
var code = currentAccount.ID + currentProduct.ProductId.ToString("0000");
```
*OR* 🡻
```csharp
var code = currentAccount.ID + GetStringProductId(currentProduct.ProductId);
private string GetStringProductId(int productId)
{
    return productId.ToString("0000");
}
```


