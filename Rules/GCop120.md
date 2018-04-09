# GCop 120

> *"Use someIntVariable.Hours() instead of TimeSpan.FromHours(someIntVariable)"*

## Rule description

It's more readable and fluent to use the `Hours()` extension method on `int`.

## Example

```csharp
Thread.Sleep(TimeSpan.FromHours(2));
```

*should be* 🡻

```csharp
Thread.Sleep(2.Hours());
```