# GCop 206

> *"Avoid using underscores in the class name"*

> *"Avoid using underscores in a local method variable declaration"*

## Rule description

Underscore should be avoided in most cases. The only exception where it's allowed (but not recommended) is for naming private class fields that back a property. 

## Example 1

```csharp
void Foo()
{
    var _bar = "someText";
}
```

*should be* 🡻

```csharp
void Foo()
{
    var bar = "someText";
}
```

## Example 2
```csharp
public class Foo_Class
{
    ...
}
```

*should be* 🡻

```csharp
public class FooClass
{
    ...
}
```

