# GCop 449

> *"Merge nested usings"*

## Rule description

To improve readability you can group multiple disposable instances of a type in one `using` statement with commas.

## Example

```csharp
using (Foo foo = new Foo())
using (Foo bar = new Foo())
{
    ...
}
```

*should be* 🡻

```csharp
using (Foo foo = new Foo(), bar = new Foo())
{
    ...
}
```

