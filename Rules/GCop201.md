# GCop201

> *"Use camelCasing when declaring local variables / a parameter."*


## Rule description

Use the following three conventions for capitalizing identifiers.

##### Pascal case

The first letter in the identifier and the first letter of each subsequent concatenated word are capitalized. You can use Pascal case for identifiers of three or more characters. For example:
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

|Identifier|Case|Example|
|---|---|---|
| Class |Pascal|AppDomain|
| Enum type|Pascal|ErrorLevel|
| Enum values|Pascal|FatalError|
| Event|Pascal|ValueChange|
| Exception class|Pascal|WebException <br> **Note**   Always ends with the suffix Exception.|
| Read-only Static field|Pascal|RedValue|
| Interface|Pascal|	IDisposable <br> **Note**   Always begins with the prefix I.|
| Method|Pascal|ToString|
| Namespace|Pascal|System.Drawing|
| Property|	Pascal|BackColor|
| Public instance field|Pascal|RedValue <br> **Note**   Rarely used. A property is preferable to using a public instance field.|
| Protected instance field|Camel|redValue <br> **Note**   Rarely used. A property is preferable to using a protected instance field.|
| Parameter|Camel|typeName|
 
