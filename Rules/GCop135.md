# GCop135

> *"Add a 'default' block after the last case in a switch statement. For example if you don't support anything other than your specified 'cases' throw a NotSupportedException"*


## Rule description
The default case can appear in any order in the switch statement. Regardless of its order in the source code, it is always evaluated last, after all case labels have been evaluated.

Reasons to use a default:

#### 1.To 'catch' an unexpected value.
If a default case is not present and the match expression does not match any other case label, program flow falls through the switch statement.

#### 2. To handle 'default' actions, where the cases are for special behavior.
You see this a lot in menu-driven programs or when a variable is declared outside the switch-case but not initialized, and each case initializes it to something different. Here the default needs to initialize it too so that down the line code that accesses the variable doesn't raise an error.

#### 3. To show someone reading your code that you've covered that case.
This was an over-simplified example, but the point is that someone reading the code shouldn't wonder why variable cannot be something other than 1 or 2.


## Example 1
**Violating code:**
```csharp
int myVar = 3;
switch(myVar)
{
    case 1:
        //something
    case 2:
        //something else 
}
```
🡻

**Compliant code**
```csharp
switch(myVar)
{
    case 1:
        //something
    case 2:
        //something else
    default:
        // unknown type! based on the language,
        // there should probably be some error-handling
        // here, maybe an exception
}
```
 

## Example 2
**Violating code:**
```csharp
int myVar;
int myValue = 3;
switch(myValue)
{
    case 1:
        myVar = 1;
        break;
    case 2:
        myVar = 2;
        break;
}
if(myVar == 4)
{
    //do something
}
```
🡻

**Compliant code**
```csharp
int myVar;
int myValue = 3;
switch(myValue)
{
    case 1:
        myVar = 1;
        break;
    case 2:
        myVar = 2;
        break;
    default:
        myVar = 4;
        break;
}
if(myVar == 4)
{
    //do something
}
```
 

## Example 3
**Violating code:**
```csharp
myVar = (myVar == "value") ? 2 : 1;
switch(myVar)
{
    case 1:
        // something
    case 2:
        // something else
}
```
🡻

**Compliant code**
```csharp
myVar = (myVar == "value") ? 2 : 1;
switch(myVar)
{
    case 1:
        // something
    case 2:
        // something else
    default:
        // will NOT execute.
}
```
 