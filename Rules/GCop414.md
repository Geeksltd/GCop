# GCop414

> *"Remove .ToString() as it's unnecessary."*


## Rule description

In many cases it is not necessory to call ToString method when using the + operator, as the C# compiler is smart enough to automatically do that. The benefits of not calling it unnecessarily are: 

1.  Avoid unnecessary clutter in your code. 
2.  To avoid null reference exception (in case the object is null)   

## Example 1

```csharp
var result = "someText" + myObject.ToString() + " ...";
```

should be 🡻

```csharp
var result = "someText" + myObject + " ...";
```

## Example 2

```csharp
public string SomeMethod() => "This year is " + DateTime.Today.Year.ToString();
```
should be 🡻

```csharp
public string SomeMethod() => "This year is " + DateTime.Today.Year;
```

## Example 3

```csharp
var result = "someText".ToString();
```
should be 🡻

```csharp
var result = "someText";
```
 
