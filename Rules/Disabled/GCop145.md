# GCop 145

> *"This phrase is translated to SQL which handles `NULL` implicitly. Do not worry about null values and write your query against the property (path) simply without using `.Get(x => x...)` or `?.`"*

## Rule description

The `Database.GetList<T>()` is translated to SQL query which handles `null` phrases. So there is no need to check null values.

## Example

```csharp
var result = await Database.GetList<myEntityName>(s => s.MyProperty == someObject?.SomeProperty);
```

*should be* 🡻

```csharp
var result = await Database.GetList<myEntityName>(s => s.MyProperty == someObject.SomeProperty);
```