# GCop 512

> *"Use \"NullableObject.Value\" instead."*

## Rule description

Casting a nullable object to its underlying type will be changed to invoking the `.Value` in the compiled IL.
Instead of casting, you should just invoke `.Value` explicitly so it's more clear and readabile.

As another benefit, simply calling `.Value` makes it easier to change the type of the variable, e.g. from `int?` to `byte?`, without breaking the code or causing strange run-time bugs as a result of a cast to an invalid type.

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
