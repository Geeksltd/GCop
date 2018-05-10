# GCop 544

> *"It should be written as : `SomeParameter = -SomeParameter`"*

## Rule description

Negating a value is much simpler than normal multiplication because there is no need to perform any actual arithmetic. It also helps to improve your code readability.

## Example

```csharp
myInteger = myInteger * -1;
```

*should be* 🡻

```csharp
myInteger = -myInteger;
```
