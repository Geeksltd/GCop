# GCop109

> *"Use ArrayName.HasMany() instead of ArrayName.Count()/ArrayName.Length()"*


## Rule description
...

## Example 1
```csharp
var myArray = new int[] { 1, 2, 3, 4, 5 };
if (myArray.Count > 1)
{
    ...
}
```
*should be* 🡻

```csharp
var myArray = new int[] { 1, 2, 3, 4, 5 };
if (myArray.HasMany())
{
    ...
}
```
 
 

## Example 2
```csharp
var myArray = new string[] { "a", "b", "c", "d" };
if (myArray.Length > 1)
{
    ...
}
```
*should be* 🡻

```csharp
var myArray = new string[] { "a", "b", "c", "d" };
if (myArray.HasMany())
{
    ...
}
```
 
 
