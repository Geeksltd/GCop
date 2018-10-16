# GCop 505

> *"Avoid interpolated string with no interpolation"*

## Rule description

The `$` special character identifies a string literal as an interpolated string. When an interpolated string is resolved to a result string, items with interpolated expressions are replaced by the string representations of the expression results. So it is redundant to use interpolated string for string without any interpolation.

## Example1

```csharp
var someString = $"";
```

*should be* 🡻

```csharp
var someString = "";
```

## Example2

```csharp
var someString = $"some string";
```

*should be* 🡻

```csharp
var someString = "some string";
```