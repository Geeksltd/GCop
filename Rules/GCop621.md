# GCop 621

> *"Drop `Where` and move the condition into the `FirstOrDefault`."*
> 
> *"Use `Cast<Foo>()` here instead."*
> 
> *"Calling `Foo` is unnecessary here."*



## Rule description

To have a more readable code, you can make some changes in `Linq` expressions such as using `FirstOrDefault()` instead of `Where` clause or `Cast` instead of casting in the select statement.

## Example1

```csharp
var details = new List<InvoiceDetail>();
...
var myList = details.Select(p => (InvoiceDetailsDto)p).ToList();
```

*should be* 🡻

```csharp
var details = new List<InvoiceDetail>();
...
var myList = details.Cast<InvoiceDetailsDto>().ToList();
```

## Example2

```csharp
var result = fooBar.where(l => l.Foo == bar).FirstOrDefault();
```

*should be* 🡻

```csharp
var result = fooBar.FirstOrDefault(l => l.Foo == bar);
```