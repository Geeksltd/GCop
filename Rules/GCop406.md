# GCop 406

> *"Mark `foo` field as read-only."*

## Rule description

When the field is not assigned or assignment to the field is just occurred as part of the declaration or in a constructor in the same class, it should be declared as a `readonly`. [Further reading](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/readonly).

## Example1

```csharp
class Foo
{
    int bar;
    public void Bar()
    {
        FooBar(bar);
    }
}
```

*should be* 🡻

```csharp
class Foo
{
    readonly int bar;
    public void Bar()
    {
        FooBar(bar);
    }
}
```

## Example2

```csharp
class Foo
{
    int bar;
    Foo(int bar)
    {
        this.bar = bar;
    }
}
```

*should be* 🡻

```csharp
class Foo
{
    readonly int bar;
    Foo(int bar)
    {
        this.bar = bar;
    }
}
```

## Example3

```csharp
private static uint timeStamp = (uint)DateTime.Now.Ticks;
///No assignment for timeStamp
```

*should be* 🡻

```csharp
private static readonly uint timeStamp = (uint)DateTime.Now.Ticks;
///No assignment for timeStamp
```

## Example4

```csharp
private int foo = 5;
///No assignment for foo
```

*should be* 🡻

```csharp
private readonly int foo = 5;
///No assignment for foo
```