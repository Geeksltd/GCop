# GCop302

> *"Since 'Idisposable object' implements IDisposable, wrap it in a **using()** statement"*


## Rule description
When you finish using an object that implements IDisposable, you should call the object's IDisposable.Dispose implementation.You can do this by **using** statement or calls the **Dispose** method.

The using statement automatically disposes the object while dispose method should be written by the programmer.Also you should check that the object is not null before dispose method.

## Example 1
```csharp
var font = new Font(pfcoll.Families[0], 58, FontStyle.Bold, GraphicsUnit.Pixel);
//several lines of code
font.Dispose();
```
*should be* 🡻

```csharp
using(var font = new Font(pfcoll.Families[0], 58, FontStyle.Bold, GraphicsUnit.Pixel))
{
    //several lines of code
}

```
