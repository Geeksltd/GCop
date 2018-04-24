# GCop 134

> *"This method should not contain complex code, Instead call other focused methods to perform the complex logic"*

## Rule description

The first rule of functions is that they should be small. Functions should do one thing. They should do it well. They should do it only, so long methods are against OOP rules and reduce the code readability.

## Example1

```csharp
private void OnSaving()
{
    //More than 10 statements
}        
```

*should be* 🡻

```csharp
private void OnSaving()
{
    //Call other methods
    //less than 10 statements
} 
//Smaller methods
```

## Example2

```csharp
protected override void OnDeleting()
{
    //More than 10 statements
}        
```

*should be* 🡻

```csharp
protected override void OnDeleting()
{
    //Call other methods
    //less than 10 statements
} 
//Smaller methods
```