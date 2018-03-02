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
 
