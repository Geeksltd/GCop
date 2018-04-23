# GCop 400

> *"Remove empty object initializer."*

## Rule description

Empty object initializers are just unnecessary noise and should be simplified.

## Example

```csharp
var customer = new Customer
{
};
```

*should be* 🡻

```csharp
var customer = new Customer();
```
