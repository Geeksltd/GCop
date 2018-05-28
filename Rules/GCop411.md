# GCop 411

> *"Do not set the ID of a new object. It's automatically set to a new Guid by the framework in the constructor."*

## Rule description

Each data row should be unique in the system and be accessed with a reference.
When you create a new Entity, M# generates by default an `ID` property, which is a Guid and not visible in the M# Entity editor. This `ID` comes from the `GuidEntity` and is used to store references to this instance in associations, or for example passing an object to another Web page using the query string. So there is no need to set set the `ID` of a new object.

## Example
>See https://github.com/Geeksltd/GCop/issues/141
```csharp
(...some violating rule)
```

*should be* 🡻

```csharp
(...corrected version)
```

## Controversy

...
