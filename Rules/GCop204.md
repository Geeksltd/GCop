# GCop204

> *"Rename the variable 'variable name' to something clear and meaningful."*


## Rule description
 Variable and parameter names should be descriptive enough that the name of the parameter and its type can be used to determine its meaning and the role of its contained data in most scenarios.
Science a single character is not meaningful at all ,this error will happen.

There are some exception in this rule:
 
  * When the parameter is type of EventArgs it can be a single character.
  * When defining the underlying private member variable for a public property.

## Example 1
```csharp
private void MyMethod(int d, EventArgs e)
{
    int m = d;
}
```
*should be* 🡻

```csharp
private void MyMethod(int date, EventArgs e)
{
    int myDate = date;
}
```
 
 

## Example 2
```csharp
public class SampleClass
{
    public string f;
    private string _n;
    public string Name
    {
        get { return this._n; }
        set { this._n = value; }
    }
}
```
*should be* 🡻

```csharp
public class SampleClass
{
    public string Family;
    private string _n;
    public string Name
    {
        get { return this._n; }
        set { this._n = value; }
    }
}
```
 
 

