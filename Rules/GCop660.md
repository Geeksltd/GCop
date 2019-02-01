# GCop 660

> *"Remove redundant assignment."*

## Rule description

Assignment of local variable or parameter just before `return` statement is redundant and useless.

## Example1

```csharp
bool Foo(bool bar)
{
    ...
    bar = false;
    return bar;
}
```

*should be* 🡻

```csharp
bool Foo(bool bar)
{
    ...
    return false;
}
```

## Example2

```csharp
bool FooBar(bool bar)
{
    if(bar)
    {
        bool foo = true;
        bar = foo;
        return bar;
    }
    ...
}
```

*should be* 🡻

```csharp
bool FooBar(bool bar)
{
    if(bar)
    {
        bool foo = true;
        return foo;
    }
    ...
}
```

