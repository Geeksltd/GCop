# GCop 405

> *"You should use {'conditional access'}"*

## Rule description

The null-condition operator `?.` help you write less code and much simpler code than using `if` statement.

## Example

```csharp
if (Results != null)
    Results.Reverse();
```

*should be* 🡻

```csharp
Results?.Reverse();
```
