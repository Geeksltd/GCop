# GCop166

> *"Avoid assignment within conditional statements"*


## Rule description
Using assignment within conditional statements would decrease readability.

This error happens while Using the expressions below within conditional statement :

* EqualsExpression
* GreaterThanExpression
* LessThanExpression
* GreaterThanOrEqualExpression
* LessThanOrEqualExpression

## Example 1
```csharp
public void MyMethod()
{
    if ((TotalRows = Query.Count()) < 1)
        //do something
}
```
*should be* 🡻

```csharp
public void MyMethod()
{
    if (CountMethod(Query, out TotalRows) < 1)
    //do something
}
public void CountMethod(IQueryable<Ticket> query, out int totalRows)
{
    return totalRows = query.Count();
}
```
 

## Example 2
```csharp
public void MyMethod()
{
    if ((TotalRows = Query.Count()) < 1)
        //do something
}
```
*should be* 🡻

```csharp
public void MyMethod()
{
    TotalRows = Query.Count();
    if ( TotalRows < 1)
        //do something
}
```
 
