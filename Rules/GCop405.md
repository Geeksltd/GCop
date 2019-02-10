# GCop 405

> *"You should use `foo?.Reverse();`"*

## Rule description

The null-condition operator `?.` help you write less code and much simpler code than using `if` statement.

## Example

```csharp
if (foo != null)
    foo.Reverse();
```

*should be* 🡻

```csharp
foo?.Reverse();
```
