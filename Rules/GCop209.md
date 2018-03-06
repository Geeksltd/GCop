# GCop209

> *"Use PascalCasing for (class or enum or ... ) names."*


## Rule description

PascalCase convention is used to capitalize some kind of identifiers in C#. In PascalCase capitalization,The first letter in the identifier and the first letter of each subsequent concatenated word are capitalized.

PascalCasing is used for these identifires :

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
 

## Example 3
```csharp
enum sampleEnum
{
    ...
}
```
*should be* 🡻

```csharp
enum SampleEnum
{
    ...
}
```
 

## Example 4
```csharp
delegate void sampleDelegateName(string str);
```
*should be* 🡻

```csharp
delegate void SampleDelegateName(string str);

```
 

## Example 5
```csharp
using sampleNameSpace;
using system.drawing;
```
*should be* 🡻

```csharp
using SampleNameSpace;
using System.Drawing;
```
 

## Example 6
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
 

## Example 7
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

## Example 8
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
 

## Example 9
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
 
