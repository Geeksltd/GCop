# GCop 175

> *"This condition is very long. Either refactor that into a method, or define interim variables to 'document' your purpose, and use the variable(s) in the IF clause."*

## Rule description

long `if` conditions are not easy to undrestand and reduce code readability. you should refactor that into a method, or define interim variables to 'document' your purpose, and use the variable(s) in the `if` clause.

## Example

```csharp
if (month > 12 || month < 1 || day > 31 || day < 1 || (month > 6 && day > 30))
{
    ...
}
```

*should be* 🡻

```csharp
if (DateTimeisNotValid(month, day))
{
    ...
}

bool DateTimeisNotValid(int month, int day)
{
    return month > 12 || month < 1 || day > 31 || day < 1 || (month > 6 && day > 30);
}
```