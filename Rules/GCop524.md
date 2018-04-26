# GCop 524

> *"Remove private keyword."*

## Rule description

`Private` is the default accessibility. When we specify no other access modifier, a member is private. So there is no need to re-write `Private` for private fileds and methods.

A private auto-property has no advantage to a simple class field. It's slightly slower to invoke compared to a plain field. It has the `{get; set;}` noise code.

## Example1

```csharp
private string MyProperty { get; set; }
```

*should be* 🡻

```csharp
public string MyProperty { get; set; }
```

*OR* 🡻

```csharp
string MyProperty;
```

## Example2

```csharp
private string MyField;
```

*should be* 🡻

```csharp
string MyField
```

## Example3

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