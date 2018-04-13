# GCop 421

> *"The conditions seems redundant. Either remove the ternary operator, or fix the values."*

## Rule description

The conditional operator returns one of two values depending on the value of a Boolean expression. If the result can be the same there would be no need to use ternary conditional operator.

It seems that such expressions are always incorrect and created as a result of a developer oversight, or when refactoring the code. It would make sense for the C# compiler to warn against that, but it doesn't unfortunately. This is why it's built into GCop.

## Example

```csharp
var cityCode = cityName == "london" ? 100 : 100;
```

*should be* 🡻

```csharp
var cityCode = cityName == "london" ? 100 : 0;
```
