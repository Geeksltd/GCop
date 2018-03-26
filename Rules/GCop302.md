# GCop302

> *"Since 'Idisposable object' implements IDisposable, wrap it in a **using()** statement"*


## Rule description
When you finish using an object that implements IDisposable, the object's Dispose method needs to be called.

If you do that manually, you need to write code to ensure that:
* The object is not null before calling Dispose()
* The dispose method is not accidentally skipped in the event of an early **return** or **break** or **continue** statement.
* In the case of exceptions, you still call the Dispose() by using a try/finally block.

Instead of doing the above manually, you can simply use a **using** block which will automatically handle all of that for you.

## Example 1
```csharp
var font = new Font(pfcoll.Families[0], 58, FontStyle.Bold, GraphicsUnit.Pixel);
...
font.Dispose();
```
*should be* 🡻

```csharp
using(var font = new Font(pfcoll.Families[0], 58, FontStyle.Bold, GraphicsUnit.Pixel))
{
    ...
}
```
