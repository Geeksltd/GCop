# GCop641

> *"Use the OrEmpty() method instead."*


## Rule description
If it's null, **OrEmpty()** return empty string. Otherwise it returns this string. Using this help you to have a more meaningful code.

## Example 1
```csharp
var myText = textBox.Text ?? string.Empty;
```
*should be* 🡻

```csharp
var myText = textBox.Text.OrEmpty();
```
