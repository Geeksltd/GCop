# GCop 643

> *"Change to return `foo ?? bar;`"*

## Rule description

When returning a value or assigning it, there are cases where you want to use a particular value, but if it's `null`, then use another value. Instead of using an `if` statement in those cases, you can use the `??` syntax.

## Example1

```csharp
if (foo != null)
    return foo;
else 
    return bar;
```

*should be* 🡻

```csharp
return foo ?? bar;
```

## Example2

```csharp
if (foo != null)
    result = foo;
else 
    result = bar;
```

*should be* 🡻

```csharp
result = foo ?? bar;
```
