# GCop 607

> *"It should be written as Enumerable.Except< TSource >"*

## Rule description

Human brain can understand positive expressions and statements faster than negative ones. To improve code readability its better to use *Except* rather than * Lambda Not Expression*. 

## Example

```csharp
double[] numbers1 = { 2.0, 2.0, 2.1, 2.2, 2.3, 2.3, 2.4, 2.5 };
double[] numbers2 = { 2.2 };

var onlyInFirstSet = numbers1.Where(a => !numbers2.Contains(a)).ToList();
```

*should be* 🡻

```csharp
double[] numbers1 = { 2.0, 2.0, 2.1, 2.2, 2.3, 2.3, 2.4, 2.5 };
double[] numbers2 = { 2.2 };

var onlyInFirstSet = numbers1.Except(numbers2).ToList();
```