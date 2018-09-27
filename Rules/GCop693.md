# GCop 693

> *"Remove empty else clause."*

## Rule description

There is no point in having empty `else` part. Unused code should be removed as it serves no purpose.

## Example

```csharp
if (someCondition)
{
    ...
}
else
{
}
```

*should be* 🡻

```csharp
if (someCondition)
{
    ...
}
```

