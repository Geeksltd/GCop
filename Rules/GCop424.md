# GCop 424

> *"This method is redundant. Callers of this method can just call \{Collection Name}.Count() which is as clean."*

## Rule description

There is no need to call a method which just return count of a collection items. Instead You can simply call the `Count()` method on that collection, which is more readable and cleaner.

## Example

```csharp
public class MyClass
{
    public IEnumerable<string> MyString { get; set; }
    
    public int MyCountMethod()
    {
        return MyString.Count();
    }
    ...
}

var countResult = MyClassObject.MyCountMethod();
```

*should be* 🡻

```csharp
public class MyClass
{
    public IEnumerable<string> MyString { get; set; }
    ...
}

var countResult = MyClassObject.MyString.Count();
```