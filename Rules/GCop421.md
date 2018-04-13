# GCop 421

> *"The conditions seems redundant. Either remove the ternary operator, or fix the values."*

## Rule description

The conditional operator returns one of two values depending on the value of a Boolean expression. If the result can be the same there would be no need to use ternary conditional operator.

## Example

```csharp
var cityCode = cityName == "london" ? 100 : 100;
```

*should be* 🡻

```csharp
var cityCode = cityName == "london" ? 100 : 0;
```