# GCop 418

> *"Remove the unnecessary casting."*

## Rule description

Duplicate casts decrease performance and is unnecessary.

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