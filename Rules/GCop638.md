# GCop 638

> *"Shorten this method by defining it as expression-bodied."*

## Rule description

Short methods that can be written in a single line should be written as expression-bodied which is a briefer and more compact format, allowing readers to see more on the screen.

## Example

```csharp
public bool Foo()
{
    return bar;
}
```

*should be* 🡻

```csharp
public bool Foo() => bar;
```