# GCop 514

> *"the method '\{MethodName}' must be private as it being used in Getter Accessor."*

## Rule description

...

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
