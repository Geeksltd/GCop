# GCop 418

> *"Remove the unnecessary casting."*

## Rule description

You should not use explicit casting code such as `(TypeX)someValue` where `someValue` is already implicitly castable to `TypeX`.
Unnecessary explicit casting is noise in code, and can even decrease performance.

## Example1

```csharp
var myVar = (int)intValue;
```

*should be* 🡻

```csharp
var myVar = intValue;
```

## Example2

```csharp
var myVar = (int)MyMethod();

public int MyMethod()
{
    ...
}
```

*should be* 🡻

```csharp
var myVar = MyMethod();

public int MyMethod()
{
    ...
}
```
