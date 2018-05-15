# GCop 309

> *"It should be written as {0}, Because the criteria will be faster and eliminate unnecessary database fetches"*

## Rule description

...

## Example

```csharp
var result = Database.GetList<Employee>(e => e == myEmployee );
```

*should be* 🡻

```csharp
var result = Database.GetList<Employee>(e => e.ID == myEmployee.ID );
```