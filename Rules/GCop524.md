# GCop 524

> *"Remove private keyword."*

## Rule description

`Private` is the default accessibility for class members. When we specify no other access modifier, a member is `private` automatically and there is no need for you to make it explicit, which adds a little bit of noise to the code.

A private auto-property has no advantage to a simple class field. It's slightly slower to invoke compared to a plain field. It has the `{get; set;}` noise code.

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

## Example 3

```csharp
private string MyProperty { get; set; }
```

*should be* 🡻

```csharp
string MyProperty;
```

