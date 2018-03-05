# GCop414

> *"Remove .ToString() as it's unnecessary."*


## Rule description

It is not necessory to call ToString method in this case because:

1.  To avoid String.ToString()

    Imagine myObj in myObj.ToString() is a string. String.ToString() Returns this instance of String. Because this method simply returns the current string unchanged, there is no need to call it.  

2.  To avoid null reference exception

    Imagine myObj in myObj.ToString() is a null reference.In this case it would throw exception.But "someText" + myObj.ToString() will not throw exception.

3.  To avoid use unnecessary method

    The + concatenation "arg0" + myObj compiles to a call to String.Concat(object arg0, object arg1). The method concatenates arg0 and arg1 by calling the ToString() method of arg0 and arg1 and String.Empty is used in place of any null argument, so there is no need to use .ToString method in this case.
    

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
 
