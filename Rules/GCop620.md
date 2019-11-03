# GCop 620

> *"To simplify the condition use the `?:`."*

## Rule description

To make short the if-else condition, we can use conditional operator `?:`. Following is the syntax for the conditional operator.

`condition ? first_expression : second_expression;`

If condition is true, first_expression is evaluated and becomes the result. If condition is false, second_expression is evaluated and becomes the result. Only one of the two expressions is evaluated.
 
## Example

```csharp
// if-else construction.  
if (foo > 0)  
    bar = "positive";  
else  
    bar = "negative";  
```

*should be* 🡻

```csharp
// ?: conditional operator.  
bar = (foo > 0) ? "positive" : "negative";
```