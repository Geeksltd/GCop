# GCop 447

> *"Avoid semicolon at the end of class declaration"*

## Rule description

The C# language wasn't designed with any C++ compatibility in mind. The language designers has simply decided, that the ending semi-colon is not needed.

## Example

```csharp
public class SampleClass
{
    ...
}; 
```

*should be* 🡻

```csharp
public class SampleClass
{
    ...
}
```

