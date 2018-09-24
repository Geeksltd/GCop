# GCop 445

> *"Remove empty statement"*

## Rule description

#### https://github.com/Geeksltd/GCop/issues/206
It is redundant to use empty statement when there is no need to perform an operation. It is very useful with a while loop with the blank body and label statements.

## Example

```csharp
public void SampleMethod()
{
    var sampleVar = 25;;
}
```

*should be* 🡻

```csharp
public void SampleMethod()
{
    var sampleVar = 25;
}
```