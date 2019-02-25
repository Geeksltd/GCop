# GCop 439

> *"Only one partial file for a class should specify the class modifiers (public, static, ...)"*

## Rule description

The following keywords on a partial-type definition are optional, but if present on one partial-type definition, cannot conflict with the keywords specified on another partial definition for the same type and there is no need to specify them again:

* `public`, `private`, `protected`, `internal`
* `abstract`, `sealed`
* base class
* generic constraints
* `new` modifier (nested parts)

Although the C# compiler allows duplicate definition of the same modifier, but to avoid confusion, only one partial class should declare all modifiers. The other partial classes should merely be `partial class Foo` without any modifier.

## Example

```csharp
public partial class FooBar
{
    public void Bar()
    {
    }
}

public partial class FooBar
{
    public void Foo()
    {
    }
}
```

*should be* 🡻

```csharp
public partial class FooBar
{
    public void Bar()
    {
    }
}

partial class FooBar
{
    public void Foo()
    {
    }
}
```
