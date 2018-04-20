# GCop 423

> *"This condition was just checked on line \{lineNumber}."*

## Rule description

Duplicate conditions are noisy, make long your code and reduce the readability of your code.
When the same condition is repeated, it's just poorly formatted logic.

## Example

```csharp
if (someBooleanExpression)
{
    DoFirstThing();
}
else
{
   ...
}

if (someBooleanExpression)
{
    DoSecondThing();
}
else
{
  ...
}
```

*should be* 🡻

```csharp
if (someBooleanExpression)
{
    DoFirstThing();
    DoSecondThing();
}
else
{
   ...
   ...
}

```
