# GCop438

> *"Blocks should not start ​or end ​with empty lines."*


## Rule description
This is just a matter of style issue. It Looks like it will reduce readability of code and makes codes too long.

## Example 1
```csharp
try
{
    ...
}
catch
{

    throw ;
}
```
*should be* 🡻

```csharp
try
{
    ...
}
catch
{
    throw ;
}
```
 

## Example 2
```csharp
public void MyMethod()
{
    ...

}
```
*should be* 🡻

```csharp
public void MyMethod()
{
    ...
}
```
 

