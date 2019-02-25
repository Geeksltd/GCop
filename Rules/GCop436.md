# GCop 436

> *"As the implementation is relatively long, change this into a standard method implementation."*

## Rule description

Expression body definitions let provide a member's implementation in a very concise, readable form, so long statements are against expression body usage and reduce code readability.

## Example

```csharp
public void Foo() => Console.WriteLine("long statement" + Bar);
```

*should be* 🡻

```csharp
public void Foo()
{
    Console.WriteLine("long statement" + Bar);
}
```