# GCop 643

> *"Change to return {something} ?? {somethingElse};"*

## Rule description

When returning a value or assigning it, there are cases where you want to use a particular value, but if it's `null`, then use another value. Instead of using an `if` statement in those cases, you can use the `??` syntax.

## Example1

```csharp
if (somethig != null)
    return something;
else 
    return somethingElse;
```

*should be* 🡻

```csharp
return something ?? somethingElse;
```

## Example2

```csharp
if (somethig != null) result = somethig;
else result = somethingElse;
```

*should be* 🡻

```csharp
result = myBoolObj ?? somethingElse;
```
