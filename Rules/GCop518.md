# GCop 518

> *"Default value of DateTime is the birth of Lord Jesus Christ, which is inappropriate for processing. Use MaxOrNull() instead."*
> 
> *"Use `MinOrNull()` {or `MaxOrNull()`} instead as the default value of {YourType(e.g.int)} is \{Default value of your type} which is not a good representation for the notion of \"unknown\"."*
> 
> *"Use `MinOrNull()` {or `MaxOrNull()`} instead of `MinOrDefault()` {or `MaxOrDefault`} and then remove the cast to \{nullableTypeCast}"*

## Rule description

The `MaxOrDefault()` and `MinOrDefault()` methods, get the maximum or minimum value of a specified expression in this list. If the list is empty, then the default value of the expression will be returned. The `MinOrNull` and `MaxOrNull` methods do the same, but they return `null`, when there is no item in the list.

These methods are useful when the default value of your type is meaningless or inappropriate.
## Example1

```csharp
var rres = someCollection.MaxOrDefault(s => s.Date);
```

*should be* 🡻

```csharp
var rres = someCollection.MaxOrNull(s => s.Date);
```

## Example2

```csharp
var rres = someCollection.MinOrDefault(s => s.Salary);
```

*should be* 🡻

```csharp
var rres = someCollection.MinOrNull(s => s.Salary);
```

## Example3

```csharp
var rres = someCollection.MaxOrDefault(s => (int?)s.Salary);
```

*should be* 🡻

```csharp
var rres = someCollection.MaxOrNull(s => s.Salary);
```
