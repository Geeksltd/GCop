# GCop 432

> *"Unnecessary paranthesis should be removed."*

## Rule description

It is possible in C# to insert parenthesis around virtually any type of expression, statement, or clause, and in many situations use of parenthesis can greatly improve the readability of the code. However, excessive use of parenthesis can have the opposite effect, making it more difficult to read and maintain the code.

## Example 1

```csharp
public int Foo()
{
    var bar = 100;
    ...
    return (bar * 10);
}
```

*should be* 🡻

```csharp
public int Foo()
{
    var bar = 100;
    ...
    return bar * 10;
}
```

## Example 2

```csharp
public void Foo(object arg)
{
    var bar = ((Bar)(arg));
}
```

*should be* 🡻

```csharp
public void Foo(object arg)
{
    var bar = (Bar)arg;
}
```

## Example 3

```csharp
public void Foo()
{
    if ((Bar() == true))
    {
        ...
    }
}
```

*should be* 🡻

```csharp
public void Foo()
{
    if (Bar() == true)
    {
        ...
    }
}
```