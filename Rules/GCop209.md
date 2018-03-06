# GCop209

> *"Use PascalCasing for (class or enum or ... ) names."*


## Rule description

PascalCase is a naming convention where he first letter in of each word in the phrase are upper case, and the rest are lower case.
You do not use under_score in PascalCased identifiers.

In C#, PascalCasing is used for these identifires:

* Classes
* Interfaces
* Enums
* Delegates 
* Namespaces
* Events
* Methods
* Enum values
* Fields
* Properties

## Example 1
```csharp
public class sampleClassName
{
    ...
}
```
*should be* 🡻

```csharp
public class SampleClassName
{
    ...
}
```

## Example 2
```csharp
interface iSampleInterface
{
    ...
}
```
*should be* 🡻

```csharp
interface ISampleInterface
{
    ...
}
```


## Example 4
```csharp
public void sampleMethod()
{
    ...
}
```
*should be* 🡻

```csharp
public void SampleMethod()
{
    ...
}
```
 

## Example 5
There is one exception where you are allowed to use underscore, and that's for event handlers.
```csharp
private void sampleButton_Click(Object sender, EventArgs e)
{
    ...
}
```
*should be* 🡻

```csharp
private void SampleButton_Click(Object sender, EventArgs e)
{
    ...
}
```
 


## Example 6
```csharp
enum SampleEnum
{
    sampleEnumValue
}
```
*should be* 🡻

```csharp
enum SampleEnum
{
    SampleEnumValue
}
```
 

## Example 7
```csharp
public class SampleClassName
{
    public static readonly string sampleStaticField = "someText";
    public string samplePublicInstanceField = "someText";
}
```
*should be* 🡻

```csharp
public class SampleClassName
{
    public static readonly string SampleStaticField = "someText";
    public string SamplePublicInstanceField = "someText";
}
```
 
## Example 10
```csharp
public class SampleClassName
{
    public string myProperty { get; set; }
}
```
*should be* 🡻

```csharp
public class SampleClassName
{
    public string MyProperty { get; set; }
}
```
 
