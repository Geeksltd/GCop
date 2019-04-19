# GCop 654

> *"Awaiting `null` will throw a runtime error. Add `?? Task.CompletedTask` after the expression."*

## Rule description

The `?.` operator returns null task instead of calling method. The null reference exception is made because you can’t await on null task. The task must be initialized. You can add `??` operator so if `?.` returns null task use `CompletedTask` instead.

## Example

```csharp
await foo?.Bar();
```

*should be* 🡻

```csharp
await (foo?.Bar() ?? Task.CompletedTask)
```