# GCop 415

> *"The same code is repeated in multiple `if` branches. Instead update the `if` condition to cover both scenarios."*

## Rule description

Repeated Statements will reduce code readability. To have a more meaningful code it is better to refactor these `if` conditions.

## Example

```csharp
if (condition1)
{
    Foo();
}
else if (condition2)
{
    //...
}
else if (condition3)
{
    Foo();
}
```

*should be* 🡻

```csharp
if (condition1 || condition3)
{
    Foo();
}
else if (condition2)
{
    //...
}
```
