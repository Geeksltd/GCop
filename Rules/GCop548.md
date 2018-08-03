# GCop 548

> *"Remove redundant As operator"*

## Rule description

The `as` operator is like a `cast` operation. However, if the conversion isn't possible, `as` returns `null`. So when a variable value is `null`, it is redundant to use `as`, because it is clear that the result is `null`.

## Example

```csharp
string someString = null;
string anotherString = someString as string;
```

*should be* 🡻

```csharp
string anotherString = someString;
```
