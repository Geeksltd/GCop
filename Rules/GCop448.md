# GCop 448

> *"Use `==` instead"*

## Rule description

The `==` works with nulls but `Equals` crashes when you compare null values.

The `==` does type checking during compile time while `Equals` is more during runtime.

## Example 1

```csharp
var result = foo.SingleOrDefault(s => s.Bar.Equals("something"));
```

*should be* 🡻

```csharp
var result = myList.SingleOrDefault(s => s.Bar == "something");
```

## Example 2

```csharp
if (foo.Equals("bar"))
{
    ...
}
```

*should be* 🡻

```csharp
if (foo == "bar")
{
    ...
}
```