﻿# GCop 514

> *"the method `CalculateFoo` must be private as it being used in Getter Accessor."*

## Rule description

If you declare the method used in Getter Accessor, as `private`, it's a statement of intent, saying I don't want these variables to be changed or accessed from the outside, especially when this is a calculater method.

## Example
>See https://github.com/Geeksltd/GCop/issues/157
```csharp
public int CustomerSalary
{
    get { return CalculateSalary(); }
    set { CustomerSalary = value; }
}
public int CalculateSalary()
{
    return SomeValue;
}
```

*should be* 🡻

```csharp
public int CustomerSalary
{
    get { return CalculateSalary(); }
    set { CustomerSalary = value; }
}
int CalculateSalary()
{
    return SomeValue;
}
```
