# GCop 418

> *"Remove the unnecessary casting."*

## Rule description

You should not use explicit casting code such as `(TypeX)someValue` where `someValue` is already implicitly castable to `TypeX`.
Unnecessary explicit casting is noise in code, and can even decrease performance.

## Example1

```csharp
void Foo(int foo)
{
    var bar = (int)foo;
}
```

*should be* 🡻

```csharp
void Foo(int foo)
{
    var bar = foo;
}
```

## Example2

```csharp
var bar = (int)Foo();

public int Foo()
{
    ...
}
```

*should be* 🡻

```csharp
var bar = Foo();

public int Foo()
{
    ...
}
```
