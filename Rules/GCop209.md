# GCop209

> *"Use PascalCasing for (class or enum or ... ) names."*


## Rule description

Use the following three conventions for capitalizing identifiers.

##### Pascal case

The first letter in the identifier and the first letter of each subsequent concatenated word are capitalized.For example:
```
BackColor
```
##### Camel case

The first letter of an identifier is lowercase and the first letter of each subsequent concatenated word is capitalized. For example:
```
backColor
```
##### Uppercase

All letters in the identifier are capitalized. Use this convention only for identifiers that consist of two or fewer letters. For example:
```
System.IO
System.Web.UI
```

## Example 1
For these identifiers we should use PascalCasing :


|Identifier|Violating code|Compliant code|
|---|---|---|
| Class |appDomain|AppDomain|
| Interface|iDisposable|IDisposable <br> **Note**   Always begins with the prefix I.|
| Enum type|errorLevel|ErrorLevel|
| delegate|performCalculation|PerformCalculation|
| Namespace|system.Drawing|System.Drawing|
| Event|valueChange|ValueChange|
| Method|toString|ToString|
| Enum values|fatalError|FatalError|
| Read-only Static field|redValue|RedValue|
| Public instance field|redValue|RedValue <br> **Note**   Rarely used. A property is preferable to using a public instance field.|
| Property|backColor|BackColor|
| Exception class|webException|WebException <br> **Note**   Always ends with the suffix Exception.|

 
