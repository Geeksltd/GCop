# GCop 107

> *"Do not use `this.Get()`. `this` is never null"*
> 
> *"Do not use `this?.ClassMember`. `this` is never null"*

## Rule description

The `this` keyword refers to the current instance of the class, so there is no need to check it for being null.

## Example1

```csharp
if (this.Get() == null)
{
    ...
}
```

*should be* 🡻

```csharp
//cpde should be deleted
```

## Example2

```csharp
var result = this?.FooProperty;
```

*should be* 🡻

```csharp
var result = this.FooProperty;
```