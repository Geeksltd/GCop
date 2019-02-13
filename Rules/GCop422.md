# GCop 422

> *"Change it to a method."*

## Rule description

Using multiple return in a `Where` clause reduce code readability. Instead you can use method to have a more meaningful and readable code.

## Example

```csharp
foo.Where(rec =>
{
    if (rec == bar)
        return true;
    return false;
});
```

*should be* 🡻

```csharp
foo.Where(rec => DecideReturnStatement(rec));
bool DecideReturnStatement(int rec, int bar)
{
    if (rec == bar)
        return true;
    return false;
}
```