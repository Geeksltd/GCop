# GCop 524

> *"Remove private keyword."*

## Rule description

Private is the default accessibility. When we specify no other access modifier, a member is private.

## Example1

```csharp
private string MyProperty { get; set; }
```

*should be* 🡻

```csharp
public string MyProperty { get; set; }
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