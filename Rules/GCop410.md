# GCop 410

> *"This method seems unnecessary as it only calls the base virtual method."*

## Rule description

Requiring an override to call the base without a template is Bad Design. It's misleading for the maintenance programmer. If you really need to do that, then there is a flaw in your design. When you don't see an override then you know the base is called. An override tells us there is something different, even if the base is called in there as well. 

Then the right thing to do is to provide a separate non-virtual method in base class and call this one instead of the virtual one.

## Example

```csharp
class BaseClass
{
    protected internal override void OnInit(EventArgs e){...}
}
public class SubClass: BaseClass
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }
}
```

*should be* 🡻

```csharp
class BaseClass
{
    protected internal override void OnInit(EventArgs e)
    {
        MyOnInitMethod();
    }
    protected void MyOnInitMethod(){...}
}
public class SubClass: BaseClass
{
    protected override void OnInit(EventArgs e)
    {
        MyOnInitMethod();
    }
}
```