# GCop 440

> *"Use StringComparison when comparing strings."*

## Rule description

The `==` Operator compares the reference identity while the `Equals()` method compares only contents of a string. So use `Equals()` when you want to test the equality rather than reference identity.

## Example1

```csharp
if (someString.ToLower() == anotherString.ToLower())
{
    ...
}
```

*should be* 🡻

```csharp
if (string.Equals(someString, anotherString, StringComparison.OrdinalIgnoreCase))
{
    ...
}
```

## Example2

```csharp
if (someString.ToLower() != anotherString.ToLower())
{
    ...
}
```

*should be* 🡻

```csharp
if (!string.Equals(someString, anotherString, StringComparison.OrdinalIgnoreCase))
{
    ...
}
```
