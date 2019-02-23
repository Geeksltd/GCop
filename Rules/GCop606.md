# GCop 606

> *"For string value existence checking use the more readable methods of .HasValue() or .IsEmpty()"*

## Rule description

It is more readable and expressive to use `.HasValue()` instead of `.Any()` and `.IsEmpty()` instead of `None()` to check a string value existence.

## Example1

```csharp
if (textBoxText.Any())
{
    ...
}
```

*should be* 🡻

```csharp
if (textBoxText.HasValue())
{
    ...
}
```

## Example2

```csharp
if (textBoxText.None())
{
    ...
}
```

*should be* 🡻

```csharp
if (textBoxText.IsEmpty())
{
    ...
}
```