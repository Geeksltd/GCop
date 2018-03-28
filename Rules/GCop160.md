# GCop160

> *"This is not readable. Either refactor into a method, or use If / else statement."*


## Rule description
Writting long conditional expression will reduce code readability and increase misundrestanding.
You should define an abstraction for the long expression. That abstraction can either be a method or a variable.

If you can't think of any good abstraction name for that concept, at least rewrite it in an *if/else *statement to make it more readable.


## Example 1
```csharp
var code = currentAccount.ID + (currentProduct.ProductId.ToString().Length > 3 ? 
       currentProduct.ProductId.ToString() : 
       (currentProduct.ProductId.ToString().Length == 2 ? "00" + currentProduct.ProductId : "0" + currentProduct.ProductId)
     );
```
*should be* 🡻

```csharp
var code = currentAccount.ID + PadProductId(currentProduct.ProductId);

string PadProductId(int productId)
{
    if (productId.ToString().Length > 3) return productId.ToString();
    else if (productId.ToString().Length == 2) return "00" + currentProduct.ProductId;
    else return "0" + currentProduct.ProductId;
}

// Note: This is for illustration only. The logic in this example could be written in a cleaner way.
```
