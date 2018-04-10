# GCop 512

> *"Use \"NullableObject.Value\" instead."*

## Rule description

A *cast expression* and `.Value` both compile to the same IL. Hard cast doesn't add anything in terms of readability, but it does cost in terms of maintainability.

If you change your variable from `int?` to `byte?`, then all your casts are wrong, but if you was using `.Value`, you are free to change the variable as necessary.

## Example

```csharp
int? myVar = 3;
int mySecondVar2 = (int)myvar;
```

*should be* 🡻

```csharp
int? myVar = 3;
int mySecondVar2 = myvar.Value;
```
