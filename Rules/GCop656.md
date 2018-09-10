# GCop 656

> *"Join string expressions"*

## Rule description

Concatenation is used to split a long string literal into smaller strings or concatenate string variables. Combining several short strings without any string variable is redundant. So to simplify we should use them as a single string without any `+` operator.

## Example

```csharp
string sample = "a" + "b";
```

*should be* 🡻

```csharp
string sample = "ab";
```
