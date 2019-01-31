# GCop 628

> *"Maybe define this method on `Foo` class as it's using \{count} of its members (compared to \{count} from this type)"*

## Rule description

The methods which have parameters of a special type and return object of that type, should be written in the class of that type. It helps the readability of the code.

## Example

```csharp
public class Foo
{
    string Param1 { get; set; }
    string Param2 { get; set; }

    private void FooMethod()
    {
        ...
        var bar = new Bar();
        bar.BarMethod(this);
    }
}

public class Bar
{
    public Foo BarMethod(Foo myFoo)
    {
        ...
        return myFoo;
    }
}
```

*should be* 🡻

```csharp
public class Foo
{
    string Param1 { get; set; }
    string Param2 { get; set; }

    private void FooMethod()
    {
        ...
        BarMethod(this);
    }
    public Foo BarMethod(Foo myFoo)
    {
        ...
        return myFoo;
    }
}
```