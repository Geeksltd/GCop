# GCop 548

> *"Remove redundant `As` operator"*

## Rule description

Redundant casts decrease performance. The `as` operation is like a `cast` operation, when an object type is declared, there is no need to cast it again.

## Example

```csharp
var foo = "";
var bar = foo as string;
```

*should be* 🡻

```csharp
var foo = "";
string bar = foo;
```
