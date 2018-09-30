# GCop 101

> *"Too many parameters as argument. Define a container and encapsulate them"*

## Rule description

More than six parameters for a method will face this hint. In fact, you should aim to reduce the number of parameters in your methods to three or less for better readability.

It is hard to understand long list of parameters. You can easily make a mistake to pass the wrong parameter in the wrong order and cause hard-to-debug issues. Instead of a long list of parameters, a method can use the data of an object that encapsulates those parameters.

If you need to pass a group of parameters together, they probably actually form a concept or abstraction that is missing from your design. So try to come up with an abstraction and give it a good name. That way, you will not only solve the immediate problem, but also potentially realize that you can refactor your code using that new class, and move some of the existing logic (methods) to that class instead. As a result you will add more clarity to your code and end up with a more object-oriented architecture.

## Example

```csharp
public void Foo(string userName, string password, string name, string family,
                     string companyPhone, string homePhone, string mobile )
{
    ...
}
```

*should be* 🡻

```csharp
public void Foo(Registration register)
{
    ...
}

public class Registration
{
    string UserName { get; set; }
    string Password { get; set; }
    string Name { get; set; }
    string Family { get; set; }
    string CompanyPhone { get; set; }
    string HomePhone { get; set; }
    string Mobile { get; set; }   
}
```

*and perhaps even*  🡻

```csharp
public class Registration
{
    string UserName { get; set; }
    string Password { get; set; }
    string Name { get; set; }
    string Family { get; set; }
    string CompanyPhone { get; set; }
    string HomePhone { get; set; }
    string Mobile { get; set; }   
    
    public void Foo()
    {
       ...
    }
}
```