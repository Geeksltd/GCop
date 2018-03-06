# GCop135

> *"Add a 'default' block after the last case in a switch statement. For example if you don't support anything other than your specified 'cases' throw a NotSupportedException"*

> *"Add descriptive comment if the default block is supposed to be empty or throw an NotSupportedException if that block is not supposed to be reached"*


## Rule description
In the Switch,Case,Default statementsts, if there is no match with a constant expression, the statement associated with the default keyword is executed. If the default keyword is not used, control passes to the statement following the switch block.

The default case can appear in any order in the switch statement. Regardless of its order in the source code, it is always evaluated last, after all case labels have been evaluated.

It is prefered to use default statement :

* To 'catch' an unexpected value.

    If a default case is not present or has an empty block and the match expression does not match any other case label, program flow falls through the switch statement.

* To handle 'default' actions, where the cases are for special behavior.
  
    it is seen a lot in menu-driven programs or when a variable is declared outside the switch-case but not initialized, and each case initializes it to something different. Here the default needs to initialize it too so that down the line code that accesses the variable doesn't raise an error.


## Example 1
In this example we don't expect **myVar** to be anything other than the cases we handle. Just in case a different value is passed at runtime, it should explicitly throw a Not Supported Exception to prevent hard-to-find bugs.
```csharp
int myVar = 3;
switch(myVar)
{
    case 1:
        //something
        break;
    case 2:
        //something else 
        break;
}
```
*should be* 🡻

```csharp
switch(myVar)
{
    case 1:
        //something
        break;
    case 2:
        //something else
        break;
    default:
        throw new NotSupportedException();        
}
```
 

## Example 2
If the default case doesn't matter and should be ignored (for when you don't have an exclusive-per-case logic) you should still add a default block to show that you have thought about it and know that it's fine if the switch results in any other unhandled case.
```csharp
switch(someEnumValue)
{
    case SomeEnum.ValueA:
        // E.g. Send a notification in this particular case;
        break;
    case SomeEnum.ValueB:
        // E.g. Do something in this special case too;
        break;
}
// ... Some logic applicable to all cases
```
*should be* 🡻

```csharp
switch(someEnumValue)
{
    case SomeEnum.ValueA:
        // E.g. Send a notification in this particular case;
        break;
    case SomeEnum.ValueB:
        // E.g. Do something in this special case too;
        break;
    default: break;
}
// ... Some logic applicable to all cases
```
 
