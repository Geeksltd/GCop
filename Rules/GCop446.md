# GCop 446

> *"Remove empty argument list from attribute"*

## Rule description

It is redundant to use an attribute with empty argument list. So we should remove empty parenthesis.

## Example

```csharp
[Obsolete()]
public void SampleMethod()
{
}
```

*should be* 🡻

```csharp
[Obsolete]
public void SampleMethod()
{
}
```

