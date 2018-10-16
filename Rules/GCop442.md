# GCop 442

> *"Use `return` instead of assignment."*

## Rule description

It is recommended to avoid useless assignment to local variable. To shorten the return code and make it more simple, return value inside condition block instead of assignment to a local variable that is not used later on, or whose value is always overwritten.

## Example

```csharp
if (condition)
{
    foo = 1;
}
else
{
    foo = 2;
}

return foo;
```

*should be* 🡻

```csharp
if (condition)
{
    return 1;
}
else
{
    return 2;
}
```
