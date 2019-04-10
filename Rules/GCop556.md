# GCop {number}

> *"Use the shortcut `.SendItemId()`"*

## Rule description

The `SendItemId` method is a shortcut for `Send("item", "item.ID")`. It helps to create a more readable code.

## Example

```csharp
Button("Bar").OnClick(x => x.Go<Foo>().Send("item", "item.ID"));
```

*should be* 🡻

```csharp
Button("Bar").OnClick(x => x.Go<Foo>().SendItemId());
```

