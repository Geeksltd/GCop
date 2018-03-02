# GCop414

> *"Remove .ToString() as it's unnecessary."*


## Rule description

Because this method simply returns the current string unchanged, there is no need to call it.

## Example 1
**Violating code:**
```csharp
var result = "someText" + myObject.ToString() + " ...";
```
🡻

**Compliant code**
```csharp
var result = "someText" + myObject + " ...";
```
 
## Example 2
**Violating code:**
```csharp
public string SomeMethod() => "This year is " + DateTime.Today.Year.ToString();
```
🡻

**Compliant code**
```csharp
public string SomeMethod() => "This year is " + DateTime.Today.Year;
```
 
