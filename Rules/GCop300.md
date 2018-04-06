# GCop300

> *"Replace with None()"*
>
> *"Replace with IsSingle()"*
> 
> *"Replace with Any()"*


## Rule description
It is more readable and fluent to use *None()* , *IsSingle()* or *Any* to determine count of collections items rather than comparing theme with zero or one, also ther are fastest options.

## Example 1
```csharp
if(myIEnumerableCollection.Count() == 0)
{
    ...
}
```
*should be* 🡻

```csharp
if(myIEnumerableCollection.None())
{
    ...
}
```

## Example 2
```csharp
if(myIEnumerableCollection.Count() == 1)
{
    ...
}
```
*should be* 🡻

```csharp
if(myIEnumerableCollection.IsSingle())
{
    ...
}
```

## Example 3
```csharp
if(myIEnumerableCollection.Count() > 0)
{
    ...
}
```
*should be* 🡻

```csharp
if(myIEnumerableCollection.Any())
{
    ...
}
```

## Example 4
```csharp
if(myIEnumerableCollection.Count() >= 1)
{
    ...
}
```
*should be* 🡻

```csharp
if(myIEnumerableCollection.Any())
{
    ...
}
```

## Example 5
```csharp
if(0 < myIEnumerableCollection.Count())
{
    ...
}
```
*should be* 🡻

```csharp
if(myIEnumerableCollection.Any())
{
    ...
}
```