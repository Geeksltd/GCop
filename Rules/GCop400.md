# GCop 400

> *"Remove empty object initializer."*

## Rule description

Empty object initializers are just unnecessary noise and should be simplified.

## Example

```csharp
var foo = new Foo {};
```

*should be* 🡻

```csharp
var foo = new Foo();
```
