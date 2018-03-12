# GCop171

> *"There is no need for calling .Value. Replace with 'Paarmeter without .Value'"*


## Rule description
There is no need to use .Value to get the value of nullable Value types parameters, unless we use these parameters in a binary expression like :

* AddExpression
* DivideExpression
* MultiplyExpression
* SubtractExpression
* ModuloExpression
* ExclusiveOrExpression

## Example 1
```csharp
public void MyMethod(DateTime? fromDate = null, DateTime? toDate = null)
{
    if(fromDate.Value > toDate.Value)
    {
        //some other codes
    }
}
```
*should be* 🡻

```csharp
public void MyMethod(DateTime? fromDate = null, DateTime? toDate = null)
{
    if(fromDate > toDate)
    {
        //some other codes
    }
}
```
