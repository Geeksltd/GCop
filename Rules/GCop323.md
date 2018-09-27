# GCop 323

> *"Simplify loop statement"*

## Rule description

To improve code readability, simplify loop statements like below examples.

## Example1

```csharp
while (true)
{
    if (condition)
    {
      SampleMethod();
    }
    else
    {
      break;
    }
}
```

*should be* 🡻

```csharp
while (condition)
{
    SampleMethod();
}
```

## Example2

```csharp
while (true)
{
    if (condition)
    {
      break;
    }    
    SampleMethod();
}
```

*should be* 🡻

```csharp
while (!condition)
{
    SampleMethod();     
}
```

## Example3

```csharp
do
{
    SampleMethod();   
    if (condition)
    {
      break;
    }    
} while (true)
```

*should be* 🡻

```csharp
do
{
  SampleMethod();
} while (!condition)
```

