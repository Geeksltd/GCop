# GCop 422

> *"Change it to a method."*

## Rule description

Using multiple return in a `Where` clause reduce code readability. Instead you can use method to have a more meaningful and readable code.

## Example

```csharp
myCollection.Where(rec =>
{
    if (rec == someValue)
        return true;
    return false;
});
```

*should be* 🡻

```csharp
myCollection.Where(rec => DecideReturnStatement(rec));
bool DecideReturnStatement(int rec)
{
    if (rec == someValue)
        return true;
    return false;
}
```