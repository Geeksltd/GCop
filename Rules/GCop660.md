# GCop 660

> *"Remove redundant assignment."*

## Rule description

Assignment of local variable or parameter just before `return` statement is redundant and useless.

## Example1

```csharp
bool SampleMethod(bool sampleArg)
{
    ...

    sampleArg = false;
    return sampleArg;
}
```

*should be* 🡻

```csharp
bool SampleMethod(bool sampleArg)
{
    ...

    return false;
}
```

## Example2

```csharp
if(someBoolVar)
{
    bool anotherBoolVar = true;
    someBoolVar = anotherBoolVar;
    return someBoolVar;
}
return false;
```

*should be* 🡻

```csharp
if(someBoolVar)
{
    var anotherBoolVar = true;
    return someBoolVar;
}
return false;
```

