# GCop 546

> *"Variables should be declared as close to their usage as possible."*

## Rule description

This is better to define variables where they are used, so there is no need to go looking around for other usages.

## Example

```csharp
void MyMethod()
{
    var someValue = 100;
    //Lines of code without using someValue
    ...
    SomeVariable = someValue;
}
```

*should be* 🡻

```csharp
void MyMethod()
{
    var someValue = 100;
    SomeVariable = someValue;
    ...
}
```