# GCop 443

> *"Remove redundant auto-property initialization"*

## Rule description

C# 6 enables you to assign an initial value for the storage used by an auto-property in the auto-property declaration. It is redundant to initialize these properties with `null`, because their value is `null` by default.

## Example

```csharp
public string Bar { get; set; } = null;
```

*should be* 🡻

```csharp
public string Bar { get; set; }

```

