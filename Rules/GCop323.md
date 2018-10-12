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
        Foo();
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
    Foo();
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
    Foo();
}
```

*should be* 🡻

```csharp
while (!condition)
{
    Foo();     
}
```

## Example3

```csharp
do
{
    Foo();   
    if (condition)
    {
      break;
    }    
} while (true);
```

*should be* 🡻

```csharp
do
{
  Foo();
} while (!condition);
```

