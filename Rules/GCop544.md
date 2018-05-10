# GCop 544

> *"It should be written as : `SomeParameter = -SomeParameter`"*

## Rule description

Negating a value is much simpler than normal multiplication because there is no need to perform any actual arithmetic. So it helps to improve your code readability.

## Example

```csharp
myParameter = myParameter * -1;
```

*should be* 🡻

```csharp
myParameter = -myParameter;
```