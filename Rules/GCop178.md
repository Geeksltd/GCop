# GCop 178

> *"Use parenthesis to clarify your boolean logic intention."*

## Rule description

Parentheses in conditionals, even if they are not strictly required, help us to write code that is clear and correct. They reduce the work required to understand the code. Conditions without parentheses are a recipe for silly mistakes.

## Example

```csharp
if (height > 200 && height > 250 || width == 500)
{
    ...
}
```

*should be* 🡻

```csharp
if (height > 200 && (height > 250 || width == 500))
{
    ...
}
```