# GCop 524

> *"Remove private keyword."*

## Rule description

`Private` is the default accessibility for class members. When we specify no other access modifier, a member is `private` automatically and there is no need for you to make it explicit, which adds a little bit of noise to the code.

## Example 1

```csharp
private string MyField;
```

*should be* 🡻

```csharp
string MyField
```

## Example 2

```csharp
private int MyInstanceMethod()
{
    ...
}
```

*should be* 🡻

```csharp
int MyInstanceMethod()
{
    ...
}
```
