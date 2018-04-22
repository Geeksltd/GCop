# GCop 176

> *"This anonymous method should not contain complex code, Instead call other focused methods to perform the complex logic"*

## Rule description

By using anonymous methods, you reduce the coding overhead in instantiating delegates because you do not have to create a separate method. To reduce the complexity of code you can use other methods.

## Example

```csharp
var myCollection = _cacheManager.Get(cacheKey, () =>
{
    ...

    foreach (var c in someCollection)
    {
        ...

        for (int i = 0; i <= someValue; i++)
        {
            ...

            if(someCondition)
            {
                ...
            }
            else
            {
                ...
            }
        }
    }
    //more than five statements
    return myModel;
});
```

*should be* 🡻

```csharp
var myCollection = _cacheManager.Get(cacheKey, () =>
{
    CallSimpleMethod();

    foreach (var c in someCollection)
    {
        CallOtherSimpleMethod(c);

        CallAnotherSimpleMetod();
    }
    return myModel;
});
```