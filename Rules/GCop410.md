# GCop 410

> *"This method seems unnecessary as it only calls the base `virtual` method."*

## Rule description

The only reason for overriding a `virtual` method is to change its behaviour. An override tells us there is something different, even if the base is called in there as well, but it should do something else also.

Therefore there is no good reason to override a method that only calls its base implementation. Such method is complete noise.

## Example

```csharp
public class Foo: BaseBar
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }
    ...
}
```

*should be* 🡻

```csharp

public class Foo: BaseBar
{
    ...
}
```

*OR* 🡻

```csharp

public class Foo: BaseBar
{
    protected override void OnInit(EventArgs e)
    {    
        base.OnInit(e); // optional
        SomeOtherLogicAlso();
    }
    ...
}
```
