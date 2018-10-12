# GCop 443

> *"Remove redundant auto-property initialization"*

## Rule description

C# 6 enables you to assign an initial value for the storage used by an auto-property in the auto-property declaration. It is redundant to initialize these properties with default value, because their value is set by default.

## Example1

```csharp
public string Bar { get; set; } = null;
```

*should be* 🡻

```csharp
public string Bar { get; set; }
```

## Example2

```csharp
public int Bar { get; set; } = 0;
```

*should be* 🡻

```csharp
public int Bar { get; set; }
```

