# GCop 147

> *"Instead of comparing the Id properties, just compare the objects directly."*

## Rule description

When you create a new Entity, M# generates by default an ID property, which is a Guid. This ID is used to store references to this instance in associations.

## Example

```csharp
var result = Database.Find<Employee>(e => e.ID == myEmployee.ID );
```

*should be* 🡻

```csharp
var result = Database.Find<Employee>(e => e.ID == myEmployee );
```
