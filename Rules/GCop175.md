# GCop 175

> *"This condition is very long. Either refactor that into a method, or define interim variables to 'document' your purpose, and use the variable(s) in the IF clause."*

## Rule description

Long `if` conditions are hard to undrestand and reduce code readability. You should either:
- Refactor that condition into a method; or
- Define interim variables to effectively *document your purpose*, and then use the variable(s) in your `if` clause.

## Example

```csharp
if (month > 12 || month < 1 || day > 31 || day < 1 || (month > 6 && day > 30))
{
    ...
}
```

*should be* 🡻

```csharp
if (IsInvalidDate(month, day))
{
    ...
}

bool IsInvalidDate(int month, int day)
{
    return month > 12 || month < 1 || day > 31 || day < 1 || (month > 6 && day > 30);
}
```

*OR* 🡻

```csharp
var isInvalidDate = month > 12 || month < 1 || day > 31 || day < 1 || (month > 6 && day > 30);
if (isInvalidDate)
{
    ...
}
```


*OR* 🡻

```csharp
var isMonthInvalid = month > 12 || month < 1;
var isDayInvalid = day > 31 || day < 1 || (month > 6 && day > 30);

if (isMonthInvalid || isDayInvalid)
{
    ...
}
```

> Note: Ignore the incorrect implementation of *days in month* in this example. :-) 
