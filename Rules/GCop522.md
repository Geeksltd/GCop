# GCop 522

> *"Avoid NullReferenceException."*

## Rule description

NullReferenceException indicates that you are trying to access member fields, or function types, on an object reference that points to null. C# 6.0 has introduced the Null-Propagation Operator (`?.`) that enables check for the null value within an object reference chain. This will return null if anything in the object reference chain is null. 

## Example

```csharp
string result = items.FirstOrDefault().ToString();
```

*should be* 🡻

```csharp
string result = items.FirstOrDefault()?.ToString();
```

