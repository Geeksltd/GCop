# GCop 620

> *"To simplify the condition use the "??"."*
>
> *"To simplify the condition use the "?:"."*

## Rule description

It is recommended to use (??) Instead of several lines of code checking null expression. The ?? operator is called the null-coalescing operator. It returns the left-hand operand if the operand is not null; otherwise it returns the right hand operand.

To make short the if-else condition, we can use conditional operator (?:). Following is the syntax for the conditional operator.


condition ? first_expression : second_expression;  

If condition is true, first_expression is evaluated and becomes the result. If condition is false, second_expression is evaluated and becomes the result. Only one of the two expressions is evaluated.

## Example 1

```csharp
var res = myObj == null ? "IsNull" : myObj;
```

*should be* 🡻

```csharp
var res = myObj ?? "IsNull"
```
 
## Example 2

```csharp
// if-else construction.  
if (input > 0)  
    classify = "positive";  
else  
    classify = "negative";  
```

*should be* 🡻

```csharp
// ?: conditional operator.  
classify = (input > 0) ? "positive" : "negative";
```